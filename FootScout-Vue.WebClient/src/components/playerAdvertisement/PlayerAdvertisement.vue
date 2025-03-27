<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useToast } from 'vue-toast-notification';
import { closeModal } from '../../services/modal/ModalFunction';
import { AccountService } from "../../services/api/AccountService";
import UserService from "../../services/api/UserService";
import PlayerAdvertisementService from "../../services/api/PlayerAdvertisementService";
import FavoritePlayerAdvertisementService from "../../services/api/FavoritePlayerAdvertisementService";
import ClubOfferService from "../../services/api/ClubOfferService";
import OfferStatusService from "../../services/api/OfferStatusService";
import PlayerPositionService from "../../services/api/PlayerPositionService";
import PlayerFootService from "../../services/api/PlayerFootService";
import ChatService from "../../services/api/ChatService";
import { TimeService } from "../../services/time/TimeService";
import type { UserDTO } from "../../models/dtos/UserDTO";
import type { ClubHistoryModel } from "../../models/interfaces/ClubHistory";
import type { PlayerAdvertisement } from "../../models/interfaces/PlayerAdvertisement";
import type { FavoritePlayerAdvertisementCreateDTO } from "../../models/dtos/FavoritePlayerAdvertisementCreateDTO";
import type { ClubOfferCreateDTO } from "../../models/dtos/ClubOfferCreateDTO";
import type { ChatCreateDTO } from "../../models/dtos/ChatCreateDTO";
import type { PlayerPosition } from "../../models/interfaces/PlayerPosition";
import type { PlayerFoot } from "../../models/interfaces/PlayerFoot";
import type { OfferStatus } from "../../models/interfaces/OfferStatus";
import '../../styles/playerAdvertisement/PlayerAdvertisement.css';

// PlayerAdvertisement.vue - Komponent obsługujący operacje na ogłoszeniach piłkarskich

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const route = useRoute();   // Pobranie informacji o aktualnej trasie (np. parametry w URL)
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Pobranie ID ogłoszenia z adresu URL
const id = Number(route.params.id);

// Definicja zmiennych przechowujących dane dotyczące użytkownika i ogłoszenia
const userId = ref<string | null>(null);
const playerAdvertisement = ref<PlayerAdvertisement | null>(null);
const player = ref<UserDTO | null>(null);
const playerClubHistories = ref<ClubHistoryModel[]>([]);
const playerAdvertisementStatus = ref<boolean | null>(null);
const favoriteId = ref<number>(0);
const offerStatusId = ref<number>(0);
const positions = ref<PlayerPosition[]>([]);
const feet = ref<PlayerFoot[]>([]);
const offerStatuses = ref<OfferStatus[]>([]);
const isAdminRole = ref<boolean | null>(null);

// Zmienne do edycji i składania ofert
const selectedClubHistory = ref<ClubHistoryModel | null>(null);
const editFormData = ref<PlayerAdvertisement | null>(null);
const createFormData = ref<ClubOfferCreateDTO>({
  playerAdvertisementId: 0,
  playerPositionId: 0,
  clubName: "",
  league: "",
  region: "",
  salary: 0,
  additionalInformation: "",
  clubMemberId: "",
});

// Obiekt przechowujący dane dotyczące ulubionego ogłoszenia piłkarskiego
const favoritePlayerAdvertisementDTO = ref<FavoritePlayerAdvertisementCreateDTO>({
    playerAdvertisementId: 0,
    userId: ''
});

// Przechowuje ID ogłoszenia do usunięcia z ulubionych
const deleteFavoriteId = ref<number | null>(null);

// Pobranie danych po zamontowaniu komponentu
onMounted(async () => {
  try {
    // Pobranie informacji o zalogowanym użytkowniku
    userId.value = await AccountService.getId();
    isAdminRole.value = await AccountService.isRoleAdmin();
    
    // Pobranie danych ogłoszenia i jego właściciela
    const data = await PlayerAdvertisementService.getPlayerAdvertisement(id);
    playerAdvertisement.value = data;
    player.value = await UserService.getUser(data.playerId);
    playerClubHistories.value = await UserService.getUserClubHistory(data.playerId);
    playerAdvertisementStatus.value = new Date(data.endDate) >= new Date();

    // Pobranie pozycji, nóg i statusów ofert
    positions.value = await PlayerPositionService.getPlayerPositions();
    feet.value = await PlayerFootService.getPlayerFeet();
    offerStatuses.value = await OfferStatusService.getOfferStatuses();

    // Pobranie dodatkowych informacji, jeśli użytkownik nie jest administratorem
    if (userId.value && !isAdminRole.value) {
      favoriteId.value = await FavoritePlayerAdvertisementService.checkPlayerAdvertisementIsFavorite(id, userId.value);
      offerStatusId.value = await ClubOfferService.getClubOfferStatusId(id, userId.value);
    }
  }
  catch (error) {
    console.error("Error fetching data:", error);
    toast.error("Failed to load data.");
  }
});

// Funkcja do uzyskania nazwy statusu oferty na podstawie ID
const getOfferStatusNameById = (id: number) => {
    const offerStatus = offerStatuses.value.find(os => os.id === id);
    return offerStatus ? offerStatus.statusName : "Unknown";
};

// Obsługa usunięcia ogłoszenia
const handleShowClubHistoryDetails = (clubHistory: ClubHistoryModel) => {
    selectedClubHistory.value = clubHistory;
};

// Pokazuje modal edycji i wypełnia formularz danymi wybranego ogłoszenia
const handleShowEditModal = (playerAdvertisement: PlayerAdvertisement) => {
    editFormData.value = playerAdvertisement;
};

// Obsługuje edycję ogłoszenia piłkarskiego
const handleEditPlayerAdvertisement = async () => {
    if (!editFormData.value) {
        toast.error("Form data is missing.");
        return;
    }

    // Walidacja danych ogłoszenia
    const validationError = validateAdvertisementForm(editFormData.value);
    if (validationError) {
        toast.error(validationError);
        return;
    }

    // Sprawdzenie poprawności pozycji i nogi piłkarza
    const position = positions.value.find(pos => pos.id === editFormData.value!.playerPositionId);
    if (!position) {
        toast.error("Invalid player position.");
        return;
    }

    const foot = feet.value.find(f => f.id === editFormData.value!.playerFootId);
    if (!foot) {
        toast.error("Invalid foot name.");
        return;
    }

    try {
        if (!editFormData.value.id) {
            toast.error("Advertisement ID is missing.");
            return;
        }

        const updatedFormData = {
            ...editFormData.value,
            playerPosition: position,
            playerFoot: foot
        };

        await PlayerAdvertisementService.updatePlayerAdvertisement(
            editFormData.value.id,
            updatedFormData
        );

        toast.success("Player advertisement updated successfully!");
        playerAdvertisement.value = await PlayerAdvertisementService.getPlayerAdvertisement(editFormData.value.id);
        
        closeModal("editPlayerAdvertisementModal");
    }
    catch (error) {
        console.error("Failed to update player advertisement:", error);
        toast.error("Failed to update player advertisement.");
    }
};

// Funkcja walidująca dane ogłoszenia
const validateAdvertisementForm = (formData: PlayerAdvertisement) => {
    const { playerPositionId, league, region, age, height, playerFootId, salaryRange } = formData;
    const { min, max } = salaryRange;

    if (!playerPositionId || !league || !region || !age || !height || !playerFootId || !min || !max)
        return 'All fields are required.';

    if (isNaN(Number(age)) || isNaN(Number(height)) || isNaN(Number(min)) || isNaN(Number(max)))
        return 'Age, height, min and max salary must be numbers.';

    if (Number(age) < 0 || Number(height) < 0 || Number(min) < 0 || Number(max) < 0)
        return 'Age, height, min and max salary must be greater than or equal to 0.';

    if (max < min) {
        return 'Max Salary must be greater than Min Salary.';
    }

    return null;
};

// Obsługuje usunięcie ogłoszenia piłkarskiego
const handleDeletePlayerAdvertisement = async () => {
    if (!playerAdvertisement.value)
        return;

    try {
        await PlayerAdvertisementService.deletePlayerAdvertisement(playerAdvertisement.value.id);
        if(isAdminRole.value === true)
        {
            toast.success('Player advertisement has been deleted successfully.');
            closeModal('deletePlayerAdvertisementModal');
            router.push('/admin/player-advertisements');
        }
        else
        {
            toast.success('Your player advertisement has been deleted successfully.');
            closeModal('deletePlayerAdvertisementModal');
            router.push('/my-player-advertisements');
        }
    }
    catch (error) {
        console.error('Failed to delete player advertisement:', error);
        toast.error('Failed to delete player advertisement.');
    }
};

// Funkcja do oznaczania ogłoszenia jako zakończone
const handleFinishPlayerAdvertisement = async () => {
    if (!playerAdvertisement.value) return;

    const position = positions.value.find(pos => pos.id === playerAdvertisement.value!.playerPositionId);
    if (!position) {
        toast.error('Invalid player position.');
        return;
    }

    const foot = feet.value.find(f => f.id === playerAdvertisement.value!.playerFootId);
    if (!foot) {
        toast.error('Invalid foot name.');
        return;
    }

    try {
        // Ustalenie aktualnej daty jako daty zakończenia
        const currentDate = new Date().toISOString();

        // Przygotowanie zaktualizowanych danych ogłoszenia
        const updatedFormData = {
            ...playerAdvertisement.value,
            playerPosition: position,
            playerFoot: foot,
            endDate: currentDate
        };

        // Aktualizacja ogłoszenia w serwisie
        await PlayerAdvertisementService.updatePlayerAdvertisement(playerAdvertisement.value.id, updatedFormData);
        closeModal('finishPlayerAdvertisementModal');

        if (isAdminRole.value) {
            toast.success('Player advertisement has been finished successfully.');
            router.push('/admin/player-advertisements');
        }
        else {
            toast.success('Your player advertisement has been finished successfully.');
            router.push('/my-player-advertisements');
        }
    } catch (error) {
        console.error('Failed to finish player advertisement:', error);
        toast.error('Failed to finish player advertisement.');
    }
};

// Obsługuje dodanie ogłoszenia do ulubionych
const handleAddToFavorite = async () => {
  if (!playerAdvertisement.value || !userId.value)
    return;

  try {
    favoritePlayerAdvertisementDTO.value.playerAdvertisementId = playerAdvertisement.value.id;
    favoritePlayerAdvertisementDTO.value.userId = userId.value;

    await FavoritePlayerAdvertisementService.addToFavorites(favoritePlayerAdvertisementDTO.value);
    toast.success('Added to favorites successfully.');
    router.push('/my-favorite-player-advertisements');
  }
  catch (error) {
    console.error('Failed to add to favorites:', error);
    toast.error('Failed to add to favorites.');
  }
};

// Funkcja do otwierania okna modalnego potwierdzającego usunięcie ogłoszenia z ulubionych
const handleShowDeleteFavoriteModal = (favoriteAdvertisementId: number) => {
    deleteFavoriteId.value = favoriteAdvertisementId;
};

// Funkcja do usunięcia ogłoszenia z ulubionych
const handleDeleteFromFavorites = async () => {
  if (!userId.value || !deleteFavoriteId.value)
    return;

  try {
    await FavoritePlayerAdvertisementService.deleteFromFavorites(deleteFavoriteId.value);
    toast.success('Removed from favorites successfully.');
    closeModal('deleteFromFavoritesModal');
    deleteFavoriteId.value = null;
    favoriteId.value = await FavoritePlayerAdvertisementService.checkPlayerAdvertisementIsFavorite(id, userId.value);
  } 
  catch (error) {
    console.error('Failed to remove from favorites:', error);
    toast.error('Failed to remove from favorites.');
  }
};

// Funkcja do składania oferty przez klub
const handleSubmitClubOffer = async () => {
    if (!userId.value)
        return;

    // Walidacja formularza oferty klubu
    const validationError = validateOfferForm(createFormData.value);
    if (validationError) {
        toast.error(validationError);
        return;
    }

    // Przygotowanie danych nowej oferty
    try {
        const newFormData: ClubOfferCreateDTO = {
            ...createFormData.value,
            playerAdvertisementId: playerAdvertisement.value?.id ?? 0,
            clubMemberId: userId.value
        };

        await ClubOfferService.createClubOffer(newFormData);
        closeModal('submitClubOfferModal');
        toast.success('The club offer was submitted successfully.');
        router.push('/my-offers-as-club');
    }
    catch (error) {
        console.error('Failed to submit club offer:', error);
        toast.error('Failed to submit club offer.');
    }
};

// Funkcja do walidacji danych w formularzu oferty klubu
const validateOfferForm = (formData: ClubOfferCreateDTO) => {
    const { playerPositionId, league, region, salary } = formData;

    if (!playerPositionId || !league || !region || !salary)
        return 'All fields are required.';

    if (isNaN(Number(salary)))
        return 'salary must be numbers.';

    if (Number(salary) < 0)
        return 'salary must be greater than or equal to 0.';

    return null;
};

// Obsługa rozpoczęcia czatu
const handleOpenChat = async () => {
  if (!playerAdvertisement.value || !userId.value)
    return;

  try {
    let chatId = await ChatService.getChatIdBetweenUsers(userId.value, playerAdvertisement.value.playerId);

    if (chatId === 0) {
      const chatCreateDTO: ChatCreateDTO = {
        user1Id: userId.value,
        user2Id: playerAdvertisement.value.playerId,
      };
      await ChatService.createChat(chatCreateDTO);
      chatId = await ChatService.getChatIdBetweenUsers(userId.value, playerAdvertisement.value.playerId);
    }
    router.push(`/chat/${chatId}`);
  }
  catch (error) {
    console.error("Failed to open chat:", error);
    toast.error("Failed to open chat.");
  }
};

</script>
<!-- Struktura strony zarządzania ogłoszeniami: obsługa edycji, usuwania, ulubionych i ofert klubowych -->
<template>
    <div class="PlayerAdvertisement" v-if="playerAdvertisement">
        <h1><i class="bi bi-person-bounding-box"></i> Player Advertisement</h1>
        <div class="ad-buttons-container mb-3">
            <template v-if="playerAdvertisementStatus">
                <div class="row g-2" v-if="playerAdvertisement.playerId === userId || isAdminRole">
                    <div class="col">
                        <button class="btn btn-warning ad-form-button" data-bs-toggle="modal" data-bs-target="#editPlayerAdvertisementModal" @click="handleShowEditModal(playerAdvertisement)">
                            <i class="bi bi-pencil-square"></i> Edit
                        </button>
                    </div>
                    <div class="col">
                        <button class="btn btn-secondary ad-form-button" data-bs-toggle="modal" data-bs-target="#finishPlayerAdvertisementModal">
                            <i class="bi bi-calendar-x"></i> Finish ad
                        </button>
                    </div>
                    <div class="col">
                        <button class="btn btn-danger ad-form-button" data-bs-toggle="modal" data-bs-target="#deletePlayerAdvertisementModal">
                            <i class="bi bi-trash"></i> Delete
                        </button>
                    </div>
                    <div class="col" v-if="isAdminRole">
                        <button class="btn btn-info ad-form-button" @click="handleOpenChat">
                            <i class="bi bi-chat-fill"></i> Chat
                        </button>
                    </div>
                </div>

                <div class="row g-2" v-else>
                    <div class="col" v-if="offerStatusId === 0">
                        <button class="btn btn-primary ad-form-button" data-bs-toggle="modal" data-bs-target="#submitClubOfferModal">
                            <i class="bi bi-pen"></i> Submit an offer
                        </button>
                    </div>
                    <div class="col" v-else>
                        <p class="status-label">
                            <i v-if="getOfferStatusNameById(offerStatusId) === 'Offered'" class="bi bi-question-diamond-fill text-purple"></i>
                            <i v-if="getOfferStatusNameById(offerStatusId) === 'Accepted'" class="bi bi-check-circle-fill text-success"></i>
                            <i v-if="getOfferStatusNameById(offerStatusId) === 'Rejected'" class="bi bi-x-circle-fill text-danger"></i>
                            {{ getOfferStatusNameById(offerStatusId) }} Offer
                        </p>
                    </div>

                    <div class="col">
                        <button v-if="favoriteId === 0" class="btn btn-success" @click="handleAddToFavorite">
                            <i class="bi bi-heart"></i>
                        </button>
                        <button v-else class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#deleteFromFavoritesModal" @click="handleShowDeleteFavoriteModal(favoriteId)">
                            <i class="bi bi-heart-fill"></i>
                        </button>
                    </div>

                    <div class="col">
                        <button class="btn btn-info" @click="handleOpenChat">
                            <i class="bi bi-chat-fill"></i>
                        </button>
                    </div>
                </div>
            </template>

            <template v-else>
                <div class="ad-status-container">
                    <p>Status: <strong>Inactive</strong></p>
                    <div class="row">
                        <div class="col" v-if="playerAdvertisement.playerId === userId || isAdminRole">
                            <button class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#deletePlayerAdvertisementModal">
                                <i class="bi bi-trash"></i> Delete
                            </button>
                        </div>
                    </div>
                </div>
            </template>
        </div>

        <div class="ad-container" v-if="playerAdvertisement && player">
        <p>
            <label class="ad-name-label">
                {{ player.firstName.toUpperCase() }} {{ player.lastName.toUpperCase() }} - {{ playerAdvertisement.playerPosition.positionName }}
            </label>
        </p>

        <div class="row">
            <div class="col">
                <label class="ad-section">CONTACT INFORMATION</label>
                <p><strong>E-mail:</strong> <span class="ad-data-label">{{ player.email }}</span></p>
                <p><strong>Phone number:</strong> <span class="ad-data-label">{{ player.phoneNumber }}</span></p>
                <p><strong>Location:</strong> <span class="ad-data-label">{{ player.location }}</span></p>
            </div>
            <div class="col">
                <label class="ad-section">PLAYER PROFILE</label>
                <p><strong>Age:</strong> <span class="ad-data-label">{{ playerAdvertisement.age }}</span></p>
                <p><strong>Height:</strong> <span class="ad-data-label">{{ playerAdvertisement.height }}</span></p>
                <p><strong>Foot:</strong> <span class="ad-data-label">{{ playerAdvertisement.playerFoot.footName }}</span></p>
            </div>
            <div class="col">
                <label class="ad-section">PREFERENCES</label>
                <p><strong>Position:</strong> <span class="ad-data-label">{{ playerAdvertisement.playerPosition.positionName }}</span></p>
                <p><strong>League (Region):</strong> <span class="ad-data-label">{{ playerAdvertisement.league }} ({{ playerAdvertisement.region }})</span></p>
                <p><strong>Salary (zł.) / month:</strong> 
                    <span class="ad-data-label">{{ playerAdvertisement.salaryRange.min }} - {{ playerAdvertisement.salaryRange.max }}</span>
                </p>
            </div>
        </div>

        <label class="ad-section">CLUB HISTORY</label>
            <div class="ad-table-responsive">
                <table class="table table-striped table-bordered table-hover">
                    <thead class="table-dark">
                        <tr>
                            <th>Date</th>
                            <th>Club</th>
                            <th>League (Region)</th>
                            <th>Position</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-if="playerClubHistories.length === 0">
                            <td colspan="5" class="text-center">No club history available</td>
                        </tr>
                        <tr v-for="(history, index) in playerClubHistories" :key="index">
                            <td>{{ TimeService.formatDateToEUR(history.startDate) }} - {{ TimeService.formatDateToEUR(history.endDate) }}</td>
                            <td>{{ history.clubName }}</td>
                            <td>{{ history.league }} ({{ history.region }})</td>
                            <td>{{ history.playerPosition.positionName }}</td>
                            <td>
                                <button class="btn btn-dark"  data-bs-toggle="modal" data-bs-target="#clubHistoryDetailsModal"  @click="handleShowClubHistoryDetails(history)">
                                    <i class="bi bi-info-square"></i>
                                </button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <p><strong>Creation Date:</strong> <span class="ad-creationDate-label">{{ TimeService.formatDateToEUR(playerAdvertisement.creationDate) }}</span></p>

            <p v-if="playerAdvertisementStatus">
                <strong>End Date (days left):</strong> 
                <span class="ad-creationDate-label">
                    {{ TimeService.formatDateToEUR(playerAdvertisement.endDate) }} 
                    ({{ TimeService.calculateDaysLeft(playerAdvertisement.endDate) }} days)
                </span>
            </p>
            <p v-else>
                <strong>End Date (days ago):</strong> 
                <span class="ad-creationDate-label">
                    {{ TimeService.formatDateToEUR(playerAdvertisement.endDate) }} 
                    ({{ TimeService.calculateSkippedDays(playerAdvertisement.endDate) }} days)
                </span>
            </p>
        </div>

        <!-- Details of Club History Modal -->
        <div class="modal" id="clubHistoryDetailsModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="clubHistoryModalLabel">Club History Details</h5>
                        <button type="button" class="btn-close" @click="closeModal('clubHistoryDetailsModal')"></button>
                    </div>
                    <div class="modal-body">
                        <div v-if="selectedClubHistory" class="modal-content-centered">
                            <p>
                                <label class="clubHistory-name-label">
                                    {{ selectedClubHistory.clubName.toUpperCase() }}
                                </label>
                            </p>
                            <p>
                                <label class="clubHistory-position-label">
                                    {{ selectedClubHistory.playerPosition.positionName }}
                                </label>
                            </p>
                            <div class="row">
                                <div class="col">
                                    <label class="clubHistory-section">CLUB INFO</label>
                                    <p><strong>League:</strong> {{ selectedClubHistory.league }}</p>
                                    <p><strong>Region:</strong> {{ selectedClubHistory.region }}</p>
                                    <p><strong>Start Date:</strong> {{ TimeService.formatDateToEUR(selectedClubHistory.startDate) }}</p>
                                    <p><strong>End Date:</strong> {{ TimeService.formatDateToEUR(selectedClubHistory.endDate) }}</p>
                                </div>
                                <div class="col">
                                    <label class="clubHistory-section">ACHIEVEMENTS</label>
                                    <p><strong>Matches:</strong> {{ selectedClubHistory.achievements.numberOfMatches }}</p>
                                    <p><strong>Goals:</strong> {{ selectedClubHistory.achievements.goals }}</p>
                                    <p><strong>Assists:</strong> {{ selectedClubHistory.achievements.assists }}</p>
                                    <p><strong>Additional Achievements:</strong> {{ selectedClubHistory.achievements.additionalAchievements }}</p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" @click="closeModal('clubHistoryDetailsModal')">Close</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Edit PlayerAdvertisement Modal -->
        <div class="modal" id="editPlayerAdvertisementModal" tabindex="-1"  v-if="editFormData">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="editPlayerAdModalLabel">Edit Player Advertisement</h5>
                        <button type="button" class="btn-close" @click="closeModal('editPlayerAdvertisementModal')"></button>
                    </div>
                    <div class="modal-body">
                        <form @submit.prevent="handleEditPlayerAdvertisement">
                            <div class="mb-3">
                                <label for="formPosition" class="form-label">Position*</label>
                                <select v-model="editFormData.playerPositionId" class="form-select">
                                    <option v-for="position in positions" :key="position.id" :value="position.id">
                                        {{ position.positionName }}
                                    </option>
                                </select>
                            </div>

                            <div class="row">
                                <div class="col">
                                    <div class="mb-3">
                                        <label for="formLeague" class="form-label">Preferred League*</label>
                                        <input type="text" class="form-control" v-model="editFormData.league" placeholder="League" maxlength="30" required>
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="mb-3">
                                        <label for="formRegion" class="form-label">Region*</label>
                                        <input type="text" class="form-control" v-model="editFormData.region" placeholder="Region" maxlength="30" required>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col">
                                    <div class="mb-3">
                                        <label for="formAge" class="form-label">Age*</label>
                                        <input type="number" class="form-control" v-model="editFormData.age" placeholder="Age" required>
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="mb-3">
                                        <label for="formHeight" class="form-label">Height (cm)*</label>
                                        <input type="number" class="form-control" v-model="editFormData.height" placeholder="Height" required>
                                    </div>
                                </div>
                            </div>

                            <div class="mb-3">
                                <label for="formFoot" class="form-label">Foot*</label>
                                <select v-model="editFormData.playerFootId" class="form-select">
                                    <option value="">Select Foot</option>
                                    <option v-for="foot in feet" :key="foot.id" :value="foot.id">
                                        {{ foot.footName }}
                                    </option>
                                </select>
                            </div>

                            <div class="row">
                                <div class="col">
                                    <div class="mb-3">
                                        <label for="formMin" class="form-label">Min Salary (zł.) / month*</label>
                                        <input type="number" class="form-control" v-model="editFormData.salaryRange.min" placeholder="Min" required>
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="mb-3">
                                        <label for="formMax" class="form-label">Max Salary (zł.) / month*</label>
                                        <input type="number" class="form-control" v-model="editFormData.salaryRange.max" placeholder="Max" required>
                                    </div>
                                </div>
                            </div>
                        </form>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" @click="closeModal('editPlayerAdvertisementModal')">Close</button>
                        <button type="button" class="btn btn-success" @click="handleEditPlayerAdvertisement">Update</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Delete Player Advertisement -->
        <div class="modal" id="deletePlayerAdvertisementModal" tabindex="-1">
            <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="deleteModalLabel">Confirm action</h5>
                    <button type="button" class="btn-close" @click="closeModal('deletePlayerAdvertisementModal')"></button>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this advertisement?
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" @click="closeModal('deletePlayerAdvertisementModal')">Cancel</button>
                    <button type="button" class="btn btn-danger" @click="handleDeletePlayerAdvertisement">Delete</button>
                </div>
            </div>
            </div>
        </div>

        <!-- Finish Player Advertisement -->
        <div class="modal" id="finishPlayerAdvertisementModal" tabindex="-1">
            <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="finishModalLabel">Confirm action</h5>
                    <button type="button" class="btn-close" @click="closeModal('finishPlayerAdvertisementModal')"></button>
                </div>
                <div class="modal-body">
                    Are you sure you want to finish this advertisement?
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" @click="closeModal('finishPlayerAdvertisementModal')">Cancel</button>
                    <button type="button" class="btn btn-dark" @click="handleFinishPlayerAdvertisement">Finish</button>
                </div>
            </div>
            </div>
        </div>

        <!-- Delete From Favorites  -->
        <div class="modal" id="deleteFromFavoritesModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="deleteFavoriteModalLabel">Confirm action</h5>
                        <button type="button" class="btn-close" @click="closeModal('deleteFromFavoritesModal')"></button>
                    </div>
                    <div class="modal-body">
                        Are you sure you want to delete this advertisement from favorites?
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" @click="closeModal('deleteFromFavoritesModal')">Cancel</button>
                        <button type="button" class="btn btn-danger" @click="handleDeleteFromFavorites">Delete</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Create Submit Club Offer Modal -->
        <div class="modal" id="submitClubOfferModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                    <h5 class="modal-title" id="submitClubOfferModalLabel">Submit Club Offer</h5>
                    <button type="button" class="btn-close" @click="closeModal('submitClubOfferModal')"></button>
                    </div>
                    <div class="modal-body">
                    <form @submit.prevent="handleSubmitClubOffer">
                        <div class="mb-3 row">
                        <label for="formPosition" class="col-sm-3 col-form-label">Position*</label>
                        <div class="col-sm-9">
                            <select class="form-select" id="formPosition" v-model="createFormData.playerPositionId">
                            <option v-for="position in positions" :key="position.id" :value="position.id">
                                {{ position.positionName }}
                            </option>
                            </select>
                        </div>
                        </div>

                        <div class="mb-3 row">
                        <label for="formClubName" class="col-sm-3 col-form-label">Club Name*</label>
                        <div class="col-sm-9">
                            <input type="text" class="form-control" id="formClubName" placeholder="Club Name" v-model="createFormData.clubName" maxlength="30" required>
                        </div>
                        </div>

                        <div class="mb-3 row">
                        <label for="formLeague" class="col-sm-3 col-form-label">League*</label>
                        <div class="col-sm-9">
                            <input type="text" class="form-control" id="formLeague" placeholder="League" v-model="createFormData.league" maxlength="30" required>
                        </div>
                        </div>

                        <div class="mb-3 row">
                        <label for="formRegion" class="col-sm-3 col-form-label">Region*</label>
                        <div class="col-sm-9">
                            <input type="text" class="form-control" id="formRegion" placeholder="Region" v-model="createFormData.region" maxlength="30" required>
                        </div>
                        </div>

                        <div class="mb-3 row">
                        <label for="formSalary" class="col-sm-3 col-form-label">Salary (zł.) / month*</label>
                        <div class="col-sm-9">
                            <input type="number" class="form-control" id="formSalary" placeholder="Salary" v-model.number="createFormData.salary" required>
                        </div>
                        </div>

                        <div class="mb-3 row">
                        <label for="formAdditionalInformation" class="col-sm-3 col-form-label">Additional Info</label>
                        <div class="col-sm-9">
                            <textarea class="form-control" id="formAdditionalInformation" placeholder="Additional Information" v-model="createFormData.additionalInformation" maxlength="200"></textarea>
                        </div>
                        </div>
                    </form>
                    </div>
                    <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" @click="closeModal('submitClubOfferModal')">Close</button>
                    <button type="submit" class="btn btn-success" @click="handleSubmitClubOffer">Submit</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>