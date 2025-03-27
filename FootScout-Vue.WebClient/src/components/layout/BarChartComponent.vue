<script setup lang="ts">
  import { defineProps, reactive } from 'vue';
  import { Chart, registerables } from 'chart.js';
  import { BarChart } from 'vue-chart-3';

  // BarCharComponent.vue - Komponent wyświetlający histogram na podstawie dostarczonych danych

  // Rejestrowanie modułów Chart.js, aby można było korzystać z jego funkcjonalności
  Chart.register(...registerables);

  // Definiujemy właściwości komponentu (props), czyli dane wejściowe przekazywane do niego
  const props = defineProps<{ chartData: any }>();

  // Definiujemy opcje konfiguracji histogramu, np. responsywność i układ osi
  const chartOptions = reactive({
    responsive: true,       // wykres dostosowuje się do rozmiaru kontenera
    maintainAspectRatio: false,     // wyłączamy utrzymanie proporcji wykresu
    scales: {
      x: { grid: { display: false } },    // ukrywamy siatkę osi X
      y: { beginAtZero: true },       // oś Y zaczyna się od zera
    },
  });

</script>

<template>
  <!-- Struktura komponentu: wykres słupkowy osadzony w divie -->
  <div>
    <!-- Komponent BarChart renderuje wykres słupkowy na podstawie przekazanych danych i opcji -->
    <BarChart :chart-data="chartData" :chart-options="chartOptions" /> <!-- wyświetl histogram -->
  </div>
</template>