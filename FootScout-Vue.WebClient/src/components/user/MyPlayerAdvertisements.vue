<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter} from "vue-router";
import { useToast } from "vue-toast-notification";
import { AccountService } from "../../services/api/AccountService";
import UserService from "../../services/api/UserService";
import { TimeService } from "../../services/time/TimeService";
import type { PlayerAdvertisement } from '../../models/interfaces/PlayerAdvertisement';
import '../../styles/user/MyPlayerAdvertisements.css';

// MyPlayerAdvertisments.vue - Komponent zarządzający ogłoszeniami piłkarskimi użytkownika

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)
const activeTab = ref("active");    // Tworzenie reaktywnej zmiennej do przechowywania aktywnej zakładki (np. aktywne ogłoszenia)

// Reaktywna zmienna przechowująca aktywne ogłoszenia piłkarskie użytkownika
const userActivePlayerAdvertisements = ref<PlayerAdvertisement[]>([]);
// Reaktywna zmienna przechowująca nieaktywne ogłoszenia piłkarskie użytkownika
const userInactivePlayerAdvertisements = ref<PlayerAdvertisement[]>([]);

// Funkcja asynchroniczna do pobierania ogłoszeń użytkownika
const fetchUserPlayerAdvertisements = async () => {
  try {
    const userId = await AccountService.getId();
    if (userId) {
      // Pobieranie aktywnych i nieaktywnych ogłoszeń użytkownika z serwisu.
      userActivePlayerAdvertisements.value = await UserService.getUserActivePlayerAdvertisements(userId);
      userInactivePlayerAdvertisements.value = await UserService.getUserInactivePlayerAdvertisements(userId);
    }
  }
  catch (error) {
    console.error("Failed to fetch user's player advertisements:", error);
    toast.error("Failed to load user’s player advertisements.");
  }
};

// Funkcja do przejścia do strony szczegółów ogłoszenia
const moveToPlayerAdvertisementPage = (playerAdvertisementId: number) => {
  router.push({
    path: `/player-advertisement/${playerAdvertisementId}`,
    state: { playerAdvertisementId },   // Przekazywanie ID ogłoszenia jako stan
  });
};

// Pobranie danych po zamontowaniu komponentu
onMounted(() => {
  fetchUserPlayerAdvertisements();
});

// Funkcja do nawigacji po aplikacji przy użyciu routera
const navigate = (path: string) => router.push(path);

</script>
<!-- Struktura strony ogłoszeń: wyświetlanie aktywnych i nieaktywnych ogłoszeń piłkarskich użytkownika -->
<template>
    <div class="MyPlayerAdvertisements">
      <h1><i class="bi bi-person-bounding-box"></i> My Player Advertisements</h1>
      
      <button class="btn btn-success mb-3" @click="navigate('/new-player-advertisement')">
        <i class="bi bi-file-earmark-plus-fill"></i> New Advertisement
      </button>
  
      <!-- Zakładki (Active / Archived) -->
      <ul class="nav nav-tabs mb-3" id="advertisementTabs">
        <li class="nav-item">
          <button class="nav-link" :class="{ active: activeTab === 'active' }" @click="activeTab = 'active'">
            Active advertisements
          </button>
        </li>
        <li class="nav-item">
          <button class="nav-link" :class="{ active: activeTab === 'archived' }" @click="activeTab = 'archived'">
            Archived advertisements
          </button>
        </li>
      </ul>
  
      <!--Active advertisements -->
      <div v-if="activeTab === 'active'">
        <h3><i class="bi bi-bookmark-check"></i> Active advertisements</h3>
        <div class="table-responsive">
          <table class="table table-striped table-bordered table-hover">
            <thead class="table-success">
              <tr>
                <th>Creation Date</th>
                <th>Position</th>
                <th>Preferred League</th>
                <th>Region</th>
                <th>Salary (zł.) / month</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="userActivePlayerAdvertisements.length === 0">
                <td colspan="6" class="text-center">No player advertisement available</td>
              </tr>
              <tr v-for="(advertisement, index) in userActivePlayerAdvertisements" :key="index">
                <td>{{ TimeService.formatDateToEUR(advertisement.creationDate) }}</td>
                <td>{{ advertisement.playerPosition.positionName }}</td>
                <td>{{ advertisement.league }}</td>
                <td>{{ advertisement.region }}</td>
                <td>{{ advertisement.salaryRange.min }} - {{ advertisement.salaryRange.max }}</td>
                <td>
                  <button class="btn btn-dark" @click="moveToPlayerAdvertisementPage(advertisement.id)">
                    <i class="bi bi-info-square"></i>
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
  
      <!--Archived advertisements -->
      <div v-if="activeTab === 'archived'">
        <h3><i class="bi bi-clipboard-x"></i> Archived advertisements</h3>
        <div class="table-responsive">
          <table class="table table-striped table-bordered table-hover">
            <thead class="table-warning">
              <tr>
                <th>End Date (days ago)</th>
                <th>Position</th>
                <th>Preferred League</th>
                <th>Region</th>
                <th>Salary (zł.) / month</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="userInactivePlayerAdvertisements.length === 0">
                <td colspan="6" class="text-center">No player advertisement available</td>
              </tr>
              <tr v-for="(advertisement, index) in userInactivePlayerAdvertisements" :key="index">
                <td>{{ TimeService.formatDateToEUR(advertisement.endDate) }} ({{ TimeService.calculateSkippedDays(advertisement.endDate) }} days)</td>
                <td>{{ advertisement.playerPosition.positionName }}</td>
                <td>{{ advertisement.league }}</td>
                <td>{{ advertisement.region }}</td>
                <td>{{ advertisement.salaryRange.min }} - {{ advertisement.salaryRange.max }}</td>
                <td>
                  <button class="btn btn-dark" @click="moveToPlayerAdvertisementPage(advertisement.id)">
                    <i class="bi bi-info-square"></i>
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
</template>