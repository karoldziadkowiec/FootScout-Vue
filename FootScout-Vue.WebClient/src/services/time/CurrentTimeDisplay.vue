<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { TimeService } from './TimeService';
import '../../styles/time/CurrentTimeDisplay.css'

// Tworzy reaktywną zmienną, która przechowuje aktualną datę i godzinę w formie tekstowej
const currentDateTime = ref<string>('');

// Funkcja aktualizująca wartość currentDateTime pobierając bieżącą datę i czas z TimeService
const updateDateTime = () => {
  currentDateTime.value = TimeService.getCurrentDateTime();
};

onMounted(() => {
  updateDateTime(); // Aktualizuje czas przy pierwszym zamontowaniu komponentu
  
  const intervalId = setInterval(updateDateTime, 1000); // Ustaw interwał, który co sekundę odświeża wartość currentDateTime

  // Po odmontowaniu komponentu zatrzymaj interwał aby zapobiec wyciekom pamięci
  onUnmounted(() => {
    clearInterval(intervalId);
  });
});
</script>

<template>
  <!-- Wyświetl aktualny czas -->
  <div class="CurrentTimeDisplay">
    {{ currentDateTime }}
  </div>
</template>