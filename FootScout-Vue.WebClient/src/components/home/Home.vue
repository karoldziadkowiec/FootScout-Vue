<script setup lang="ts">
import { onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import VueScrollTo from 'vue-scrollto';
import '../../styles/home/Home.css';

// Home.vue - Komponent głównej strony aplikacji z nawigacją i przewijaniem

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const route = useRoute();   // Pobranie informacji o aktualnej trasie (np. parametry w URL)
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Ładowanie danych po załadowaniu komponentu (onMounted)
// Sprawdza, czy w parametrach URL znajduje się wiadomość toast
// Jeśli tak, wyświetla powiadomienie użytkownikowi
onMounted(() => {
  if (route.query.toastMessage) {
    toast.success(route.query.toastMessage as string);
  }
});

// Funkcja do przewijania strony do wybranej sekcji na podstawie jej ID
const scrollToSection = (id: string) => {
  VueScrollTo.scrollTo(`#${id}`, {
    duration: 600,
    easing: 'ease-in-out',
  });
};

// Funkcja do nawigacji do innej podstrony w aplikacji
const moveToPage = (page: string) => {
  router.push(`/${page}`);
};

</script>
<!-- Struktura strony głównej: sekcje dla zawodników i klubów -->
<template>
  <div class="Home">
    <section id="home" class="startSection">
      <div class="Home-logo-container">
        <img src="../../assets/logo.png" alt="Home-logo" class="Home-logo" />
        FootScout
      </div>
      <h2>YOUR BEST WEBSITE TO MANAGE FOOTBALL TRANSFERS</h2>
      <h4>Discover player offers and find the best team/player for you.</h4>
      <div class="links">
        <a href="#" @click.prevent="scrollToSection('forPlayers')" class="link">For Players</a>
        <div class="sign"> | </div>
        <a href="#" @click.prevent="scrollToSection('forClubs')" class="link">For Clubs</a>
      </div>
    </section>

    <!-- Sekcja dla zawodników -->
    <section id="forPlayers" class="blackSection">
      <h1><i class="bi bi-person-bounding-box"></i> For Players</h1>
      <h5>Create a new ad as a player looking for a club and collect offers from clubs.</h5>
      <div class="container links">
        <div class="row">
          <div class="col">
            <button class="btn btn-success dynamic-button" @click="moveToPage('new-player-advertisement')">
              New Advertisement
            </button>
          </div>
        </div>
      </div>
    </section>

    <!-- Sekcja dla klubów -->
    <section id="forClubs" class="whiteSection">
      <h1><i class="bi bi-shield-fill"></i> For Clubs</h1>
      <h5>Find your dream player for club by searching ads and through recommendations.</h5>
      <div class="container links">
        <div class="row">
          <div class="col">
            <button class="btn btn-success dynamic-button" @click="moveToPage('player-advertisements')">
              Advertisements
            </button>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>