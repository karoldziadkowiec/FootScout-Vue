<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import ChatService from '../../services/api/ChatService';
import MessageService from '../../services/api/MessageService';
import { TimeService } from '../../services/time/TimeService';
import BarChartComponent from '../layout/BarChartComponent.vue';
import '../../styles/admin/AdminRaportsChats.css';

// AdminRaportsChats.vue - Komponent zarządzający statystykami czatów i wiadomości administratora

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Definiowanie stanu do przechowywania liczby czatów.
const chatCount = ref<number>(0);
// Definiowanie stanu do przechowywania danych o wysłanych wiadomościach.
const messageSendData = ref<{ date: string; count: number }[]>([]);
// Definiowanie stanu do przechowywania bieżącego miesiąca.
const currentMonth = ref<Date>(new Date());

// Ładowanie danych o liczbie czatów oraz wiadomościach po załadowaniu komponentu
onMounted(async () => {
  try {
    const _chatCount = await ChatService.getChatCount();
    const _messages = await MessageService.getAllMessages();

    if (_messages.length > 0) {
      // Obliczenie daty pierwszej wiadomości oraz bieżącej daty w formacie YYYY-MM-DD
      const firstUserDate = new Date(Math.min(..._messages.map(msg => new Date(msg.timestamp).getTime()))).toISOString().split('T')[0];
      const todayDate = new Date().toISOString().split('T')[0];

      // Generowanie zakresu dat od pierwszej wiadomości do dzisiaj
      const dateRange = TimeService.generateDateRange(firstUserDate, todayDate);
      const creationCounts: Record<string, number> = {};

      // Przeliczanie liczby wiadomości na każdy dzień
      _messages.forEach(msg => {
        const date = new Date(msg.timestamp).toISOString().split('T')[0];
        creationCounts[date] = (creationCounts[date] || 0) + 1;
      });

      // Przygotowanie danych do wyświetlenia na wykresie, posortowanych według dat
      messageSendData.value = dateRange.map(date => ({
        date,
        count: creationCounts[date] || 0,
      })).sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());

      // Ustawienie liczby czatów w stanie
      chatCount.value = _chatCount;   
    }
  }
  catch (error) {
    console.error('Failed to fetch chat data:', error);
    toast.error('Failed to load chat data.');
  }
});

// Funkcja zmieniająca miesiąc o określoną wartość (w przód lub w tył)
const changeMonth = (direction: number) => {
  currentMonth.value = new Date(currentMonth.value.getFullYear(), currentMonth.value.getMonth() + direction, 1);
};

// Filtrowanie danych, aby pokazać tylko wiadomości z bieżącego miesiąca
const messageSendFilteredData = computed(() => {
  return messageSendData.value.filter(item => item.date.startsWith(TimeService.formatDateToMonth(currentMonth.value)));
});

// Przygotowanie danych dla wykresu słupkowego
const chartData = computed(() => ({
  labels: messageSendFilteredData.value.map(item => item.date),   // Etykiety dla osi X (daty)
  datasets: [
    {
      label: 'Messages Sent',
      backgroundColor: '#8884d8',
      data: messageSendFilteredData.value.map(item => item.count),    // Dane do wykresu (liczba wiadomości)
    },
  ],
}));

// Funkcja eksportująca dane czatów do pliku CSV
const exportDataToCSV = async () => {
  await ChatService.exportChatsToCsv();
};

</script>
<!-- Struktura strony admina: wyświetlanie statystyk czatów, wykresów i możliwość eksportu danych -->
<template>
  <div class="AdminRaportsChats">
    <h1><i class="bi bi-chat-text-fill"></i> Chats - Reports & Stats</h1>
    <h3>Chat rooms count: <strong>{{ chatCount }}</strong></h3>

    <button class="btn btn-primary me-2" @click="router.push('/admin/chats')">
      <i class="bi bi-info-circle"></i> Show Chats
    </button>
    <button class="btn btn-success" @click="exportDataToCSV">
      <i class="bi bi-download"></i> Export to CSV
    </button>

    <h5>Message send dates for {{ TimeService.formatDateToMonth(currentMonth) }}</h5>
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