<script setup lang="ts">
import DynamicNavbar from './components/layout/DynamicNavbar.vue';
import ToastContainer from 'vue-toast-notification';
import './style.css';

// Główna komponentowa struktura Vue, w której osadzany jest router-view (dynamiczne strony).

</script>

<template>
  <div class="App">
  <!-- Pasek nawigacyjny na samej górze strony dla usera/admina albo w ogóle, 
   który zmienia się dynamicznie w zależności od roli użytkownika -->
    <DynamicNavbar>
      <!-- Główne miejsce na dynamicznie zmieniające się widoki aplikacji -->
      <router-view></router-view>
    </DynamicNavbar>
  </div>
</template>

<!--  Główna rola: Definiuje układ aplikacji, w tym dynamiczną nawigację (DynamicNavbar) i główną 
  przestrzeń na zmieniające się widoki (router-view).-->