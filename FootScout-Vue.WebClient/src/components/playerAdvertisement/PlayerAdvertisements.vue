<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import { closeModal  } from '../../services/modal/ModalFunction';
import { TimeService } from '../../services/time/TimeService';
import { AccountService } from '../../services/api/AccountService';
import UserService from '../..//services/api/UserService';
import PlayerPositionService from '../..//services/api/PlayerPositionService';
import PlayerAdvertisementService from '../..//services/api/PlayerAdvertisementService';
import FavoritePlayerAdvertisementService from '../..//services/api/FavoritePlayerAdvertisementService';
import type { PlayerPosition } from '../..//models/interfaces/PlayerPosition';
import type { PlayerAdvertisement } from '../..//models/interfaces/PlayerAdvertisement';
import type { FavoritePlayerAdvertisement } from '../..//models/interfaces/FavoritePlayerAdvertisement';
import type { FavoritePlayerAdvertisementCreateDTO } from '../..//models/dtos/FavoritePlayerAdvertisementCreateDTO';
import '../../styles/playerAdvertisement/PlayerAdvertisements.css';

// PlayerAdvertisments.vue - Komponent zarządzający wyświetlaniem ogłoszeń piłkarskich i ulubionych ogłoszeń użytkownika

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Zmienne do przechowywania danych w komponentach
const userId = ref<string | null>(null);
const isAdminRole = ref<boolean | null>(null);    // Flaga sprawdzająca, czy użytkownik ma rolę administratora
const positions = ref<PlayerPosition[]>([]);
const playerAdvertisements = ref<PlayerAdvertisement[]>([]);
const favoritePlayerAdvertisements = ref<FavoritePlayerAdvertisement[]>([]);
const favoritePlayerAdvertisementIds = ref<number[]>([]);
const deleteFavoriteId = ref<number | null>(null);      // ID ogłoszenia do usunięcia z ulubionych

// Obiekt do przechowywania danych potrzebnych do dodania ogłoszenia do ulubionych
const favoritePlayerAdvertisementDTO = ref<FavoritePlayerAdvertisementCreateDTO>({
  playerAdvertisementId: 0,
  userId: ''
});

// Zmienne do obsługi wyszukiwania i filtrowania
const searchTerm = ref<string>('');               // Termin wyszukiwania
const selectedPosition = ref<string | ''>('');    // Wybrana pozycja gracza do filtrowania
const sortCriteria = ref('creationDateDesc');     // Kryterium sortowania ogłoszeń

// Zmienne do obsługi paginacji
const currentPage = ref<number>(1);
const itemsPerPage = 20;

// Funkcja wywoływana po zamontowaniu komponentu, pobierająca dane użytkownika, pozycje oraz ogłoszenia piłkarskie
onMounted(async () => {
  await fetchUserData();
  await fetchPositions();
  await fetchPlayerAdvertisements();
  if (userId.value) {
    await fetchFavoritePlayerAdvertisements();
  }
});

// Obserwator na zmiany userId, który powoduje pobranie ulubionych ogłoszeń po zalogowaniu użytkownika
watch(userId, async (newUserId) => {
  if (newUserId) {
    await fetchFavoritePlayerAdvertisements();
  }
});

// Funkcja do pobierania danych użytkownika (ID i rola administratora)
const fetchUserData = async () => {
  try {
    const id = await AccountService.getId();
    if (id) {
      userId.value = id;
      isAdminRole.value = await AccountService.isRoleAdmin();
    }
  }
  catch (error) {
    console.error('Failed to fetch user data:', error);
    toast.error('Failed to load user data.');
  }
}

// Funkcja do pobierania dostępnych pozycji graczy
const fetchPositions = async () => {
  try {
    positions.value = await PlayerPositionService.getPlayerPositions();
  }
  catch (error) {
    console.error('Failed to fetch positions:', error);
    toast.error('Failed to load positions.');
  }
}

// Funkcja do pobierania aktywnych ogłoszeń piłkarskich
const fetchPlayerAdvertisements = async () => {
  try {
    playerAdvertisements.value = await PlayerAdvertisementService.getActivePlayerAdvertisements();
  }
  catch (error) {
    console.error('Failed to fetch player advertisements:', error);
    toast.error('Failed to load player advertisements.');
  }
}

// Funkcja do pobierania ulubionych ogłoszeń użytkownika
const fetchFavoritePlayerAdvertisements = async () => {
  if (!userId.value)
    return;

  try {
    const favorites = await UserService.getUserActivePlayerAdvertisementFavorites(userId.value);
    favoritePlayerAdvertisements.value = favorites;
    favoritePlayerAdvertisementIds.value = favorites.map(fav => fav.playerAdvertisementId);
  }
  catch (error) {
    console.error('Failed to fetch favorite player advertisements:', error);
    toast.error('Failed to load favorite player advertisements.');
  }
}

// Funkcja zmieniająca stronę w paginacji
const handlePageChange = (pageNumber: number) => {
    currentPage.value = pageNumber;
};

// Funkcja do przejścia na stronę szczegółów ogłoszenia piłkarskiego
const moveToPlayerAdvertisementPage = (playerAdvertisementId: number) => {
  router.push({ path: `player-advertisement/${playerAdvertisementId}`, state: { id: playerAdvertisementId } });
}

// Funkcja do wyświetlenia modalu potwierdzenia usunięcia ulubionego ogłoszenia
const handleShowDeleteFavoriteModal = (favoriteAdvertisementId: number) => {
  deleteFavoriteId.value = favoriteAdvertisementId;
}

// Funkcja do pobrania ID ulubionego ogłoszenia
const getFavoriteId = (advertisementId: number): number | null => {
  const favorite = favoritePlayerAdvertisements.value.find(fav => fav.playerAdvertisementId === advertisementId);
  return favorite ? favorite.id : null;
}

// Funkcja do usuwania ogłoszenia z ulubionych
const deleteFromFavorites = async () => {
  if (!userId.value || !deleteFavoriteId.value)
    return;

  try {
    await FavoritePlayerAdvertisementService.deleteFromFavorites(deleteFavoriteId.value);
    toast.success('Advertisement removed from favorites.');
    deleteFavoriteId.value = null;
    await fetchPlayerAdvertisements();
    await fetchFavoritePlayerAdvertisements();
    closeModal('deleteFavoriteModal');
  }
  catch (error) {
    console.error('Failed to delete favorite:', error);
    toast.error('Failed to delete advertisement from favorites.');
  }
}

// Funkcja do dodawania ogłoszenia do ulubionych
const handleAddToFavorite = async (playerAdvertisement: PlayerAdvertisement) => {
  if (!playerAdvertisement || !userId.value)
    return;

  try {
    const createFormData = { ...favoritePlayerAdvertisementDTO.value, playerAdvertisementId: playerAdvertisement.id, userId: userId.value };
    await FavoritePlayerAdvertisementService.addToFavorites(createFormData);
    toast.success('Advertisement added to favorites.');
    await fetchPlayerAdvertisements();
    await fetchFavoritePlayerAdvertisements();
  }
  catch (error) {
    console.error('Failed to add favorite:', error);
    toast.error('Failed to add advertisement to favorites.');
  }
}

// Funkcja filtrująca ogłoszenia na podstawie wyszukiwanego terminu (computed properties)
const searchAdvertisements = computed(() => {
  if (!searchTerm.value)
    return playerAdvertisements.value;

  const lowerCaseSearchTerm = searchTerm.value.toLowerCase();
  return playerAdvertisements.value.filter(ad =>
    (ad.player.firstName + ' ' + ad.player.lastName).toLowerCase().includes(lowerCaseSearchTerm) ||
    ad.league.toLowerCase().includes(lowerCaseSearchTerm) ||
    ad.region.toLowerCase().includes(lowerCaseSearchTerm)
  );
});

// Funkcja filtrująca ogłoszenia na podstawie wybranej pozycji
const filterAdvertisementsByPosition = computed(() => {
  if (!selectedPosition.value)
    return searchAdvertisements.value;

  return searchAdvertisements.value.filter(ad => ad.playerPosition.id === parseInt(selectedPosition.value, 10));
});

// Funkcja sortująca ogłoszenia według wybranego kryterium
const sortedAdvertisements = computed(() => {
  return [...filterAdvertisementsByPosition.value].sort((a, b) => {
    switch (sortCriteria.value) {
      case 'creationDateAsc':
        return new Date(a.creationDate).getTime() - new Date(b.creationDate).getTime();
      case 'creationDateDesc':
        return new Date(b.creationDate).getTime() - new Date(a.creationDate).getTime();
      case 'positionAsc':
        return a.playerPosition.positionName.localeCompare(b.playerPosition.positionName);
      case 'positionDesc':
        return b.playerPosition.positionName.localeCompare(a.playerPosition.positionName);
      case 'leagueAsc':
        return a.league.localeCompare(b.league);
      case 'leagueDesc':
        return b.league.localeCompare(a.league);
      case 'regionAsc':
        return a.region.localeCompare(b.region);
      case 'regionDesc':
        return b.region.localeCompare(a.region);
      case 'salaryAsc':
        return a.salaryRange.min - b.salaryRange.min;
      case 'salaryDesc':
        return b.salaryRange.min - a.salaryRange.min;
      default:
        return 0;
    }
  });
});

// Funkcja do obliczania liczby stron w paginacji
const totalPages = computed(() => Math.ceil(sortedAdvertisements.value.length / itemsPerPage));

// Funkcja do pobierania ogłoszeń do wyświetlenia na bieżącej stronie
const currentPlayerAdvertisementItems = computed(() => {
  const indexOfLastItem = currentPage.value * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  return sortedAdvertisements.value.slice(indexOfFirstItem, indexOfLastItem);
});

</script>
<!-- Struktura strony z ogłoszeniami piłkarskimi: wyszukiwanie, filtrowanie, paginacja i interakcje z ulubionymi ogłoszeniami -->
<template>
    <div class="PlayerAdvertisements">
      <h1><i class="bi bi-list-nested"></i> Player Advertisements</h1>
      <button class="btn btn-success form-button" @click="router.push('/new-player-advertisement')">
        <i class="bi bi-file-earmark-plus-fill"></i> New Advertisement
      </button>
      <p></p>
  
      <div class="d-flex align-items-center mb-3">
        <!-- Wyszukiwanie -->
        <div>
          <label class="form-label"><strong>Search</strong></label>
          <input
            type="text"
            class="form-control"
            placeholder="Search"
            v-model="searchTerm"
          />
        </div>
  
        <!-- Filtrowanie -->
        <div class="ms-auto">
          <label class="form-label"><strong>Filter Positions</strong></label>
          <select class="form-select" v-model="selectedPosition">
            <option value="">All Positions</option>
            <option v-for="position in positions" :key="position.id" :value="position.id">
              {{ position.positionName }}
            </option>
          </select>
        </div>
  
        <!-- Sortowanie -->
        <div class="ms-auto">
          <label class="form-label"><strong>Sort by</strong></label>
          <select class="form-select" v-model="sortCriteria">
            <option value="creationDateAsc">Creation Date Ascending</option>
            <option value="creationDateDesc">Creation Date Descending</option>
            <option value="positionAsc">Position Ascending</option>
            <option value="positionDesc">Position Descending</option>
            <option value="leagueAsc">League Ascending</option>
            <option value="leagueDesc">League Descending</option>
            <option value="regionAsc">Region Ascending</option>
            <option value="regionDesc">Region Descending</option>
            <option value="salaryAsc">Salary Ascending</option>
            <option value="salaryDesc">Salary Descending</option>
          </select>
        </div>
      </div>
  
      <div class="table-responsive">
        <table class="table table-striped table-bordered table-hover table-light">
          <thead class="table-dark">
            <tr>
              <th>Creation Date</th>
              <th>Player</th>
              <th>Position</th>
              <th>Preferred League</th>
              <th>Region</th>
              <th>Salary (zł.) / month</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="currentPlayerAdvertisementItems.length === 0">
              <td colspan="7" class="text-center">No player advertisement available</td>
            </tr>
            <tr v-for="advertisement in currentPlayerAdvertisementItems" :key="advertisement.id">
              <td class="ad-row">{{ TimeService.formatDateToEUR(advertisement.creationDate) }}</td>
              <td class="ad-row">{{ advertisement.player.firstName }} {{ advertisement.player.lastName }}</td>
              <td class="ad-row">{{ advertisement.playerPosition.positionName }}</td>
              <td class="ad-row">{{ advertisement.league }}</td>
              <td class="ad-row">{{ advertisement.region }}</td>
              <td class="ad-row">{{ advertisement.salaryRange.min }} - {{ advertisement.salaryRange.max }}</td>
              <td class="ad-row">
                <button class="btn btn-dark button-spacing" @click="moveToPlayerAdvertisementPage(advertisement.id)">
                  <i class="bi bi-info-square"></i>
                </button>
                <template v-if="advertisement.playerId !== userId && !isAdminRole">
                  <button
                    v-if="favoritePlayerAdvertisementIds.includes(advertisement.id)"
                    class="btn btn-danger"
                    data-bs-toggle="modal"
                    data-bs-target="#deleteFavoriteModal"
                    @click="() => { 
                      const favoriteId = getFavoriteId(advertisement.id);
                      if (favoriteId !== null) handleShowDeleteFavoriteModal(favoriteId);
                    }"
                  >
                    <i class="bi bi-heart-fill"></i>
                  </button>
                  <button v-else class="btn btn-success" @click="handleAddToFavorite(advertisement)">
                    <i class="bi bi-heart"></i>
                  </button>
                </template>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
  
      <!-- Paginacja -->
      <div class="pagination-container">
        <ul class="pagination pagination-green">
          <li class="page-item" :class="{ disabled: currentPage === 1 }">
            <button class="page-link" @click="handlePageChange(currentPage - 1)">Previous</button>
          </li>
          <li v-for="page in totalPages" :key="page" class="page-item" :class="{ active: page === currentPage }">
            <button class="page-link" @click="handlePageChange(page)">{{ page }}</button>
          </li>
          <li class="page-item" :class="{ disabled: currentPage === totalPages }">
            <button class="page-link" @click="handlePageChange(currentPage + 1)">Next</button>
          </li>
        </ul>
      </div>
  
      <!-- Delete Favorite Player Advertisement Modal -->
      <div class="modal" id="deleteFavoriteModal" tabindex="-1">
        <div class="modal-dialog" role="document">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">Confirm action</h5>
              <button type="button" class="btn-close" @click="closeModal('deleteFavoriteModal')"></button>
            </div>
            <div class="modal-body">
              Are you sure you want to delete this advertisement from favorites?
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" @click="closeModal('deleteFavoriteModal')">Cancel</button>
              <button type="button" class="btn btn-danger" @click="deleteFromFavorites">Delete</button>
            </div>
          </div>
        </div>
      </div>
    </div>
</template>