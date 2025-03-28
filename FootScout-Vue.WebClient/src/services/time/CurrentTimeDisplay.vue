<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { TimeService } from './TimeService';
import '../../styles/time/CurrentTimeDisplay.css'

// CurrentTimeDisplay.vue - Komponent wyświetlający aktualny czas z aktualizacją co sekundę

// Tworzy reaktywną zmienną, która przechowuje aktualną datę i godzinę w formie tekstowej
const currentDateTime = ref<string>('');

// Funkcja, która pobiera bieżącą datę i czas z `TimeService` i aktualizuje zmienną `currentDateTime`
const updateDateTime = () => {
  currentDateTime.value = TimeService.getCurrentDateTime();
};

// Funkcja wywoływana po zamontowaniu komponentu. Służy do inicjalizacji logiki w komponencie
onMounted(() => {
  updateDateTime(); // Pierwsze pobranie i wyświetlenie czasu zaraz po zamontowaniu komponentu
  
  const intervalId = setInterval(updateDateTime, 1000); // Ustaw interwał, który co sekundę odświeża wartość currentDateTime

  // Po odmontowaniu komponentu zatrzymaj interwał aby zapobiec wyciekom pamięci
  onUnmounted(() => {
    clearInterval(intervalId);
  });
});

</script>
<!-- Wyświetlanie aktualnego czasu w komponencie, zaktualizowanego co sekundę -->
<template>
  <!-- Wyświetl aktualny czas -->
  <div class="CurrentTimeDisplay">
    {{ currentDateTime }}   <!-- Powiązanie wartości zmiennej `currentDateTime` z wyświetlaną treścią na stronie -->
  </div>
</template>