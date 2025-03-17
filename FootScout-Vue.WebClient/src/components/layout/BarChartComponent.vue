<script setup lang="ts">
  import { defineProps, reactive } from 'vue';
  import { Chart, registerables } from 'chart.js';
  import { BarChart } from 'vue-chart-3';

  // Rejestrowanie modułów Chart.js
  Chart.register(...registerables);

  const props = defineProps<{ chartData: any }>();

  // opcje histogramu
  const chartOptions = reactive({
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      x: { grid: { display: false } },
      y: { beginAtZero: true },
    },
  });
</script>

<template>
  <div>
    <BarChart :chart-data="chartData" :chart-options="chartOptions" /> <!-- wyświetl histogram -->
  </div>
</template>