<script setup lang="ts">
import DynamicNavbar from './components/layout/DynamicNavbar.vue';
import ToastContainer from 'vue-toast-notification';
import './style.css';
</script>

<template>
  <div class="App">
  <!-- Navbar na samej górze strony dla usera/admina albo w ogóle -->
    <DynamicNavbar>
      <router-view></router-view> <!-- Dynamiczne strony aplikacji -->
    </DynamicNavbar>
  </div>
</template>