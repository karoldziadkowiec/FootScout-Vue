<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue';
import { useRoute } from 'vue-router';
import { AccountService } from '../../services/api/AccountService';
import { Role } from '../../models/enums/Role';
import NavbarComponent from '../../components/layout/Navbar.vue';
import AdminNavbarComponent from '../../components/layout/AdminNavbar.vue';

// DynamicNavbar.vue - Komponent dynamicznie wybierający odpowiedni navbar w zależności od roli użytkownika

// Zmienna przechowująca rolę użytkownika (może być Admin, User lub null, jeśli nie pobrano)
const role = ref<Role | null>(null);

const route = useRoute();   // Pobranie informacji o aktualnej trasie (np. parametry w URL)

// Funkcja asynchroniczna pobierająca rolę użytkownika z API
const fetchRole = async () => {
  role.value = await AccountService.getRole(); // pobierz rolę
};

// Pobranie roli użytkownika przy pierwszym załadowaniu komponentu
onMounted(fetchRole);

// Nasłuchiwanie zmiany trasy i ponowne pobranie roli użytkownika w razie potrzeby
watch(() => route.path, fetchRole);

// Obliczona wartość sprawdzająca, czy użytkownik jest na stronie logowania lub rejestracji
const isAuthPage = computed(() => route.path === '/' || route.path === '/registration');

</script>
<!-- Struktura nawigacji dynamicznie dostosowującej się do użytkownika -->
<template>
  <div>
    <!-- Ukryj navbar, jeśli użytkownik jest na stronie logowania lub rejestracji -->
    <template v-if="!isAuthPage">
      <!-- Jeśli użytkownik ma rolę administratora, wyświetl AdminNavbarComponent -->
      <AdminNavbarComponent v-if="role === Role.Admin" />
      <!-- Jeśli użytkownik jest zwykłym użytkownikiem, wyświetl standardowy NavbarComponent -->
      <NavbarComponent v-else-if="role === Role.User" />
    </template>

    <!-- Slot umożliwiający osadzenie innych komponentów w tym layoucie -->
    <slot></slot>
  </div>
</template>