<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue';
import { useRoute } from 'vue-router';
import { AccountService } from '../../services/api/AccountService';
import { Role } from '../../models/enums/Role';
import NavbarComponent from '../../components/layout/Navbar.vue';
import AdminNavbarComponent from '../../components/layout/AdminNavbar.vue';

const role = ref<Role | null>(null);
const route = useRoute(); // Pobierz aktualną trasę

const fetchRole = async () => {
  role.value = await AccountService.getRole();
};

// Pobierz rolę przy zamontowaniu komponentu
onMounted(fetchRole);

// Aktualizuj rolę przy każdej zmianie trasy
watch(() => route.path, fetchRole);

// Sprawdź, czy jesteś na stronie logowania lub rejestracji
const isAuthPage = computed(() => route.path === '/' || route.path === '/registration');
</script>

<template>
  <div>
    <!-- Ukryj navbar, jeśli użytkownik jest na stronie logowania lub rejestracji -->
    <template v-if="!isAuthPage">
      <AdminNavbarComponent v-if="role === Role.Admin" />
      <NavbarComponent v-else-if="role === Role.User" />
    </template>

    <slot></slot> <!-- Renderowanie stron wewnątrz layoutu -->
  </div>
</template>