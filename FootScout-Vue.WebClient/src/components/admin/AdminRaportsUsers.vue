<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import UserService from '../../services/api/UserService';
import { TimeService } from '../../services/time/TimeService';
import BarChartComponent from '../layout/BarChartComponent.vue';
import '../../styles/admin/AdminRaportsUsers.css';

// AdminRaportsUsers.vue - Komponent odpowiedzialny za wyświetlanie raportów użytkowników i statystyk

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Reaktywne zmienne do przechowywania liczby użytkowników oraz danych o ich tworzeniu
const userCount = ref<number>(0);
const userCreationData = ref<{ date: string; count: number }[]>([]);
const currentMonth = ref<Date>(new Date());   // Aktualnie wybrany miesiąc

// Pobieranie danych po zamontowaniu komponentu
onMounted(async () => {
  try {
    // Pobranie listy użytkowników z API
    const _users = await UserService.getUsers();

    if (_users.length > 0) {
      // Znalezienie daty pierwszego użytkownika i ustawienie zakresu dat
      const firstUserDate = new Date(Math.min(..._users.map(user => new Date(user.creationDate).getTime()))).toISOString().split('T')[0];
      const todayDate = new Date().toISOString().split('T')[0];

      // Zliczanie liczby użytkowników utworzonych w poszczególnych dniach
      const dateRange = TimeService.generateDateRange(firstUserDate, todayDate);
      const creationCounts: Record<string, number> = {};
      _users.forEach(user => {
        const creationDate = new Date(user.creationDate).toISOString().split('T')[0];
        creationCounts[creationDate] = (creationCounts[creationDate] || 0) + 1;
      });

      // Tworzenie danych dla wykresu w odpowiednim formacie
      userCreationData.value = dateRange.map(date => ({
        date,
        count: creationCounts[date] || 0,
      })).sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());

      // Ustawienie łącznej liczby użytkowników
      userCount.value = _users.length;
    }
  }
  catch (error) {
    console.error('Failed to fetch user data:', error);
    toast.error('Failed to load user data.');
  }
});

// Funkcja do zmiany aktualnie wybranego miesiąca w raportach
const changeMonth = (direction: number) => {
  currentMonth.value = new Date(currentMonth.value.getFullYear(), currentMonth.value.getMonth() + direction, 1);
};

// Filtrowanie danych, aby wyświetlać tylko użytkowników utworzonych w wybranym miesiącu
const usersFilteredData = computed(() => {
  return userCreationData.value.filter(item => item.date.startsWith(TimeService.formatDateToMonth(currentMonth.value)));
});

// Przygotowanie danych do wykresu słupkowego
const chartData = computed(() => ({
  labels: usersFilteredData.value.map(item => item.date),
  datasets: [
    {
      label: 'User Creation Count',
      backgroundColor: '#8884d8',
      data: usersFilteredData.value.map(item => item.count),
    },
  ],
}));

// Eksportowanie danych użytkowników do pliku CSV
const exportDataToCSV = async () => {
  await UserService.exportUsersToCsv();
};

// Nawigacja do strony zarządzania użytkownikami
const navigateToUsers = () => {
  router.push('/admin/users');
};

</script>
<!-- Struktura strony admina: raporty dotyczące użytkowników i statystyki -->
<template>
  <div class="AdminRaportsUsers">
    <h1><i class="bi bi-people-fill"></i> Users - Reports & Stats</h1>
    <h3>Users count: <strong>{{ userCount }}</strong></h3>

    <button class="btn btn-primary me-2" @click="navigateToUsers">
      <i class="bi bi-info-circle"></i> Show Users
    </button>
    <button class="btn btn-success" @click="exportDataToCSV">
      <i class="bi bi-download"></i> Export to CSV
    </button>

    <h5>Creation dates for {{ TimeService.formatDateToMonth(currentMonth) }}</h5>
    <div class="histogram-container">
      <BarChartComponent :chart-data="chartData" />
    </div>

    <button class="btn btn-dark me-2" @click="changeMonth(-1)">
      <i class="bi bi-arrow-left"></i>
    </button>
    <button class="btn btn-dark" @click="changeMonth(1)">
      <i class="bi bi-arrow-right"></i>
    </button>
  </div>
</template>