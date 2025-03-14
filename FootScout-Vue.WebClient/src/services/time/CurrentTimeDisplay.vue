<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { TimeService } from './TimeService';
import '../../styles/time/CurrentTimeDisplay.css'

const currentDateTime = ref<string>('');

const updateDateTime = () => {
  currentDateTime.value = TimeService.getCurrentDateTime();
};

onMounted(() => {
  updateDateTime();
  const intervalId = setInterval(updateDateTime, 1000);

  onUnmounted(() => {
    clearInterval(intervalId);
  });
});
</script>

<template>
  <div class="CurrentTimeDisplay">
    {{ currentDateTime }}
  </div>
</template>