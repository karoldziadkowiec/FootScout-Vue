<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import PlayerAdvertisementService from '../../services/api/PlayerAdvertisementService';
import { TimeService } from '../../services/time/TimeService';
import BarChartComponent from '../layout/BarChartComponent.vue';
import '../../styles/admin/AdminRaportsPlayerAdvertisements.css';

// AdminRaportsPlayerAdvertisements.vue - Komponent generujący raporty i statystyki dotyczące ogłoszeń graczy

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Liczba ogłoszeń graczy
const playerAdvertisementCount = ref<number>(0);
// Dane o ogłoszeniach (data utworzenia + liczba)
const playerAdvertisementCreationData = ref<{ date: string; count: number }[]>([]);
// Aktualnie wybrany miesiąc
const currentMonth = ref<Date>(new Date());

// Pobieranie danych po zamontowaniu komponentu
onMounted(async () => {
  try {
    // Pobranie wszystkich ogłoszeń
    const _playerAdvertisements = await PlayerAdvertisementService.getAllPlayerAdvertisements();

    if (_playerAdvertisements.length > 0) {
      // Pobranie najwcześniejszej i dzisiejszej daty
      const firstUserDate = new Date(Math.min(..._playerAdvertisements.map(ad => new Date(ad.creationDate).getTime()))).toISOString().split('T')[0];
      const todayDate = new Date().toISOString().split('T')[0];

      // Generowanie zakresu dat między pierwszym a dzisiejszym ogłoszeniem
      const dateRange = TimeService.generateDateRange(firstUserDate, todayDate);
      const creationCounts: Record<string, number> = {};

      // Zliczanie ogłoszeń dla każdej daty
      _playerAdvertisements.forEach(ad => {
        const creationDate = new Date(ad.creationDate).toISOString().split('T')[0];
        creationCounts[creationDate] = (creationCounts[creationDate] || 0) + 1;
      });

      // Tworzenie uporządkowanej tablicy danych do wykresu
      playerAdvertisementCreationData.value = dateRange.map(date => ({
        date,
        count: creationCounts[date] || 0,
      })).sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());

      playerAdvertisementCount.value = _playerAdvertisements.length;  // Ustawienie całkowitej liczby ogłoszeń
    }
  }
  catch (error) {
    console.error('Failed to fetch player advertisement data:', error);
    toast.error('Failed to load player advertisement data.');
  }
});

// Zmiana aktualnie wybranego miesiąca
const changeMonth = (direction: number) => {
  currentMonth.value = new Date(currentMonth.value.getFullYear(), currentMonth.value.getMonth() + direction, 1);
};

// Filtrowanie danych wykresu dla aktualnie wybranego miesiąca
const playerAdvertisementFilteredData = computed(() => {
  return playerAdvertisementCreationData.value.filter(item => item.date.startsWith(TimeService.formatDateToMonth(currentMonth.value)));
});

// Dane do wykresu słupkowego
const chartData = computed(() => ({
  labels: playerAdvertisementFilteredData.value.map(item => item.date),
  datasets: [
    {
      label: 'Advertisements Created',
      backgroundColor: '#8884d8',
      data: playerAdvertisementFilteredData.value.map(item => item.count),
    },
  ],
}));

// Eksport danych do pliku CSV
const exportDataToCSV = async () => {
  await PlayerAdvertisementService.exportPlayerAdvertisementsToCsv();
};

</script>
<!-- Struktura strony admina: raporty i statystyki ogłoszeń graczy -->
<template>
  <div class="AdminRaportsPlayerAdvertisements">
    <h1><i class="bi bi-person-bounding-box"></i> Player Advertisements - Reports & Stats</h1>
    <h3>Player Advertisements count: <strong>{{ playerAdvertisementCount }}</strong></h3>

    <button class="btn btn-primary me-2" @click="router.push('/admin/player-advertisements')">
      <i class="bi bi-info-circle"></i> Show Player Advertisements
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