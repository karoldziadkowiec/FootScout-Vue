<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import ClubOfferService from '../../services/api/ClubOfferService';
import { TimeService } from '../../services/time/TimeService';
import BarChartComponent from '../layout/BarChartComponent.vue';
import '../../styles/admin/AdminRaportsClubOffers.css';

// AdminRaportsClubOffers.vue - Komponent wyświetlający raporty i statystyki ofert klubów

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Zmienna przechowująca liczbę ofert klubów
const clubOfferCount = ref<number>(0);
// Zmienna przechowująca dane o liczbie ofert z dnia na dzień
const clubOffersCreationData = ref<{ date: string; count: number }[]>([]);
// Zmienna przechowująca aktualny miesiąc, który jest wyświetlany
const currentMonth = ref<Date>(new Date());

// Załadowanie danych po zamontowaniu komponentu
onMounted(async () => {
  try {
    // Pobranie wszystkich ofert klubów z serwisu API
    const _clubOffers = await ClubOfferService.getClubOffers();

    if (_clubOffers.length > 0) {

      // Wyznaczenie najwcześniejszej daty oferty oraz dzisiejszej daty
      const firstUserDate = new Date(Math.min(..._clubOffers.map(offer => new Date(offer.creationDate).getTime()))).toISOString().split('T')[0];
      const todayDate = new Date().toISOString().split('T')[0];

      // Generowanie zakresu dat od najstarszej do dzisiejszej
      const dateRange = TimeService.generateDateRange(firstUserDate, todayDate);
      const creationCounts: Record<string, number> = {};    // Obiekt do zliczania liczby ofert na dany dzień

      // Iterowanie po ofertach i zliczanie ich według daty
      _clubOffers.forEach(offer => {
        const creationDate = new Date(offer.creationDate).toISOString().split('T')[0];
        creationCounts[creationDate] = (creationCounts[creationDate] || 0) + 1;
      });

      // Mapowanie zakresu dat do obiektów z datą i liczbą ofert
      clubOffersCreationData.value = dateRange.map(date => ({
        date,
        count: creationCounts[date] || 0,
      })).sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());

      clubOfferCount.value = _clubOffers.length;
    }
  }
  catch (error) {
    console.error('Failed to fetch club offer data:', error);
    toast.error('Failed to load club offer data.');
  }
});

// Funkcja do zmiany wyświetlanego miesiąca (przesuwanie w lewo lub prawo)
const changeMonth = (direction: number) => {
  currentMonth.value = new Date(currentMonth.value.getFullYear(), currentMonth.value.getMonth() + direction, 1);
};

// Wyliczanie danych o ofertach dla aktualnego miesiąca
const clubOffersFilteredData = computed(() => {
  return clubOffersCreationData.value.filter(item => item.date.startsWith(TimeService.formatDateToMonth(currentMonth.value)));
});

// Przygotowanie danych do wykresu – etykiety to daty, a dane to liczba ofert
const chartData = computed(() => ({
  labels: clubOffersFilteredData.value.map(item => item.date),
  datasets: [
    {
      label: 'Club Offers Created',
      backgroundColor: '#8884d8',
      data: clubOffersFilteredData.value.map(item => item.count),   // Liczba ofert w danej dacie
    },
  ],
}));

// Funkcja eksportu danych do pliku CSV
const exportDataToCSV = async () => {
  await ClubOfferService.exportClubOffersToCsv();
};

</script>
<!-- Struktura strony admina: raporty ofert klubów, wykres i opcje eksportu danych -->
<template>
  <div class="AdminRaportsClubOffers">
    <h1><i class="bi bi-briefcase-fill"></i> Club Offers - Reports & Stats</h1>
    <h3>Club Offers count: <strong>{{ clubOfferCount }}</strong></h3>

    <button class="btn btn-primary me-2" @click="router.push('/admin/club-offers')">
      <i class="bi bi-info-circle"></i> Show Club Offers
    </button>
    <button class="btn btn-success" @click="exportDataToCSV">
      <i class="bi bi-download"></i> Export to CSV
    </button>

    <h5>Offer send dates for {{ TimeService.formatDateToMonth(currentMonth) }}</h5>
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