<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import { closeModal  } from '../../services/modal/ModalFunction';
import { TimeService } from '../../services/time/TimeService';
import { AccountService } from '../../services/api/AccountService';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import ClubOfferService from '../../services/api/ClubOfferService';
import ChatService from '../../services/api/ChatService';
import type { PlayerPosition } from '../../models/interfaces/PlayerPosition';
import type { ClubOffer } from '../../models/interfaces/ClubOffer';
import type { ChatCreateDTO } from '../../models/dtos/ChatCreateDTO';
import '../../styles/admin/AdminClubOffers.css';

// AdminClubOffers.vue - Komponent zarządzający ofertami klubów, umożliwiający ich przeglądanie, filtrowanie, sortowanie oraz usuwanie

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Zmienne używane w komponencie
const userId = ref<string | null>(null);            // Id użytkownika (może być null, jeśli nie uda się pobrać)
const positions = ref<PlayerPosition[]>([]);        // Lista pozycji zawodników
const clubOffers = ref<ClubOffer[]>([]);            // Lista ofert klubów
const selectedOffer = ref<ClubOffer | null>(null);  // Wybrana oferta
const deleteOfferId = ref<number | null>(null);     // Id oferty do usunięcia

// Zmienne do wyszukiwania, filtrowania i sortowania
const searchTerm = ref<string>('');
const selectedStatus = ref<string>('all');
const selectedPosition = ref<string>('');
const sortCriteria = ref<string>('creationDateDesc');

// Zmienne do paginacji
const currentPage = ref<number>(1);
const itemsPerPage = 20;

// Załadowanie danych po zamontowaniu komponentu
onMounted(async () => {
    try {
        userId.value = await AccountService.getId();
        positions.value = await PlayerPositionService.getPlayerPositions();
        clubOffers.value = await ClubOfferService.getClubOffers();
    }
    catch (error) {
        console.error('Error loading data:', error);
        toast.error('Failed to load data.');
    }
});

// Funkcja obsługująca zmianę strony
const handlePageChange = (pageNumber: number) => {
  currentPage.value = pageNumber;
};

// Funkcja przenosząca do strony ogłoszenia zawodnika
const moveToPlayerAdvertisementPage = (playerAdvertisementId: number) => {
    router.push({ path: `/player-advertisement/${playerAdvertisementId}`, state: { playerAdvertisementId } });
};

// Funkcja wyświetlająca szczegóły oferty
const handleShowOfferDetails = (clubOffer: ClubOffer) => {
    selectedOffer.value = clubOffer;
};

// Funkcja wyświetlająca modal do potwierdzenia usunięcia oferty
const handleShowDeleteModal = (offerId: number) => {
    deleteOfferId.value = offerId;
};

// Funkcja do usunięcia oferty
const deleteOffer = async () => {
    if (!deleteOfferId.value)
        return;
    try {
        await ClubOfferService.deleteClubOffer(deleteOfferId.value);
        toast.success("Offer deleted successfully.");
        clubOffers.value = await ClubOfferService.getClubOffers();  // Odświeżenie listy ofert
        closeModal('deleteOfferModal');
    }
    catch (error) {
        console.error('Failed to delete offer:', error);
        toast.error('Failed to delete offer.');
    }
};

// Funkcja otwierająca czat z klubem
const handleOpenChat = async (receiverId: string) => {
    if (!receiverId || !userId.value)
        return;

    try {
        let chatId = await ChatService.getChatIdBetweenUsers(userId.value, receiverId);  // Sprawdzenie, czy czat istnieje
        if (chatId === 0) {
            const chatCreateDTO: ChatCreateDTO = { user1Id: userId.value, user2Id: receiverId };
            await ChatService.createChat(chatCreateDTO);  // Utworzenie nowego czatu
            chatId = await ChatService.getChatIdBetweenUsers(userId.value, receiverId);  // Pobranie id czatu
        }
        router.push({ path: `/chat/${chatId}`, state: { chatId } });   // Przeniesienie do strony czatu
    }
    catch (error) {
        console.error('Failed to open chat:', error);
        toast.error('Failed to open chat.');
    }
};

// Funkcja filtrująca oferty na podstawie wyszukiwania
const searchedOffers = computed(() =>
    clubOffers.value.filter(offer => {
        if (!searchTerm.value) return true;     // Jeśli brak wyszukiwania, wszystkie oferty są dopuszczalne
        const lowerCaseSearch = searchTerm.value.toLowerCase();
        return (
            `${offer.clubMember.firstName} ${offer.clubMember.lastName}`.toLowerCase().includes(lowerCaseSearch) ||
            `${offer.playerAdvertisement.player.firstName} ${offer.playerAdvertisement.player.lastName}`.toLowerCase().includes(lowerCaseSearch) ||
            offer.clubName.toLowerCase().includes(lowerCaseSearch) ||
            offer.league.toLowerCase().includes(lowerCaseSearch) ||
            offer.region.toLowerCase().includes(lowerCaseSearch)
        );
    })
);

// Funkcja filtrująca oferty po statusie
const filteredOffersByStatus = computed(() =>
    searchedOffers.value.filter(offer => {
        if (selectedStatus.value === 'all') return true;
        const isActive = new Date(offer.playerAdvertisement.endDate).getTime() >= Date.now();
        return selectedStatus.value === 'active' ? isActive : !isActive;
    })
);

// Funkcja filtrująca oferty po pozycji
const filteredOffersByPosition = computed(() =>
    selectedPosition.value
        ? filteredOffersByStatus.value.filter(offer => offer.playerPosition.id === parseInt(selectedPosition.value, 10))
        : filteredOffersByStatus.value
);

// Funkcja sortująca oferty na podstawie wybranego kryterium
const sortedOffers = computed(() =>
    [...filteredOffersByPosition.value].sort((a, b) => {
        switch (sortCriteria.value) {
            case 'creationDateAsc': return new Date(a.creationDate).getTime() - new Date(b.creationDate).getTime();
            case 'creationDateDesc': return new Date(b.creationDate).getTime() - new Date(a.creationDate).getTime();
            case 'positionAsc': return a.playerPosition.positionName.localeCompare(b.playerPosition.positionName);
            case 'positionDesc': return b.playerPosition.positionName.localeCompare(a.playerPosition.positionName);
            case 'clubNameAsc': return a.clubName.localeCompare(b.clubName);
            case 'clubNameDesc': return b.clubName.localeCompare(a.clubName);
            case 'leagueAsc': return a.league.localeCompare(b.league);
            case 'leagueDesc': return b.league.localeCompare(a.league);
            case 'regionAsc': return a.region.localeCompare(b.region);
            case 'regionDesc': return b.region.localeCompare(a.region);
            case 'salaryAsc': return a.salary - b.salary;
            case 'salaryDesc': return b.salary - a.salary;
            default: return 0;
        }
    })
);

// Funkcja obliczająca oferty na bieżącej stronie
const currentClubOfferItems = computed(() => {
    const startIndex = (currentPage.value - 1) * itemsPerPage;
    return sortedOffers.value.slice(startIndex, startIndex + itemsPerPage);
});

// Funkcja obliczająca całkowitą liczbę stron w paginacji
const totalPages = computed(() => Math.ceil(sortedOffers.value.length / itemsPerPage));

</script>
<!-- Struktura strony admina: lista ofert klubowych, wyszukiwanie, filtrowanie, sortowanie i paginacja -->
<template>
    <div class="AdminClubOffers">
            <h1><i class="bi bi-briefcase-fill"></i> Club Offers</h1>
            <p></p>

            <div class="d-flex align-items-center mb-3">
            <!-- Wyszukiwanie ofert -->
            <div>
                <label class="form-label"><strong>Search</strong></label>
                <input 
                type="text" 
                class="form-control" 
                placeholder="Search" 
                v-model="searchTerm" 
                />
            </div>

            <!-- Filtrowanie po statusie -->
            <div class="ms-auto">
                <label class="form-label"><strong>Ad Status</strong></label>
                <select class="form-select" v-model="selectedStatus">
                <option value="all">All</option>
                <option value="active">Active</option>
                <option value="inactive">Inactive</option>
                </select>
            </div>

            <!-- Filtrowanie po pozycji -->
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
                <option value="clubNameAsc">Club Name Ascending</option>
                <option value="clubNameDesc">Club Name Descending</option>
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
                    <th>Ad Status</th>
                    <th>Offer Status</th>
                    <th>Club Member</th>
                    <th>Club</th>
                    <th>Player</th>
                    <th>Position</th>
                    <th></th>
                </tr>
                </thead>
                <tbody>
                <tr v-if="currentClubOfferItems.length === 0">
                    <td colspan="8" class="text-center">No club offer available</td>
                </tr>
                <tr v-else v-for="(offer, index) in currentClubOfferItems" :key="index">
                    <td class="offer-row">{{ TimeService.formatDateToEUR(offer.creationDate) }}</td>
                    <td class="offer-row">
                    <i 
                        :class="new Date(offer.playerAdvertisement.endDate).getTime() >= Date.now() ? 'bi bi-check-circle-fill text-success' : 'bi bi-x-circle-fill text-danger'"
                    ></i>
                    {{ new Date(offer.playerAdvertisement.endDate).getTime() >= Date.now() ? 'Active' : 'Inactive' }}
                    </td>
                    <td class="offer-row">
                    <i v-if="offer.offerStatus.statusName === 'Offered'" class="bi bi-question-diamond-fill" style="color: #b571ff"></i>
                    <i v-else-if="offer.offerStatus.statusName === 'Accepted'" class="bi bi-check-circle-fill text-success"></i>
                    <i v-else-if="offer.offerStatus.statusName === 'Rejected'" class="bi bi-x-circle-fill text-danger"></i>
                    {{ offer.offerStatus.statusName }}
                    </td>
                    <td class="offer-row">{{ offer.clubMember.firstName }} {{ offer.clubMember.lastName }}</td>
                    <td class="offer-row">{{ offer.clubName }}</td>
                    <td class="offer-row">{{ offer.playerAdvertisement.player.firstName }} {{ offer.playerAdvertisement.player.lastName }}</td>
                    <td class="offer-row">{{ offer.playerPosition.positionName }}</td>
                    <td class="offer-row">
                    <button class="btn btn-primary me-1" data-bs-toggle="modal" data-bs-target="#offerDetailsModal" @click="handleShowOfferDetails(offer)">
                        <i class="bi bi-info-circle"></i> Offer
                    </button>
                    <button class="btn btn-danger me-1" data-bs-toggle="modal" data-bs-target="#deleteOfferModal" @click="handleShowDeleteModal(offer.id)">
                        <i class="bi bi-trash"></i>
                    </button>
                    <span class="button-spacing">|</span>
                    <button class="btn btn-dark me-1" @click="moveToPlayerAdvertisementPage(offer.playerAdvertisementId)">
                        <i class="bi bi-info-square"></i> Ad
                    </button>
                    <span v-if="offer.clubMemberId !== userId">
                        <span class="button-spacing">|</span>
                        <button class="btn btn-info" @click="handleOpenChat(offer.clubMemberId)">
                        <i class="bi bi-chat-fill"></i>
                        </button>
                    </span>
                    </td>
                </tr>
                </tbody>
            </table>
        </div>

        <!-- Modal szczegółów oferty -->
        <div class="modal" id="offerDetailsModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Offer Details</h5>
                    <button type="button" class="btn-close" @click="closeModal('offerDetailsModal')"></button>
                </div>
                <div class="modal-body">
                    <div v-if="selectedOffer">
                    <p><strong>Offered Date:</strong> {{ TimeService.formatDateToEUR(selectedOffer.creationDate) }}</p>
                    <p><strong>Club Name:</strong> {{ selectedOffer.clubName }}</p>
                    <p><strong>League:</strong> {{ selectedOffer.league }}</p>
                    <p><strong>Region:</strong> {{ selectedOffer.region }}</p>
                    <p><strong>Salary (zł./month):</strong> {{ selectedOffer.salary }}</p>
                    </div>
                </div>
                <div class="modal-footer">
                    <button class="btn btn-secondary" @click="closeModal('offerDetailsModal')">Close</button>
                </div>
                </div>
            </div>
        </div>

        <!-- Modal usuwania oferty -->
        <div class="modal" id="deleteOfferModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Confirm action</h5>
                    <button type="button" class="btn-close" @click="closeModal('deleteOfferModal')"></button>
                </div>
                <div class="modal-body">Are you sure you want to delete this offer?</div>
                <div class="modal-footer">
                    <button class="btn btn-secondary" @click="closeModal('deleteOfferModal')">Cancel</button>
                    <button class="btn btn-danger" @click="deleteOffer">Delete</button>
                </div>
                </div>
            </div>
        </div>

        <!-- Paginacja -->
        <nav class="admin-pagination-container">
            <ul class="pagination pagination-blue">
                <li class="page-item" :class="{ disabled: currentPage === 1 }">
                <button class="page-link" @click="handlePageChange(currentPage - 1)">Previous</button>
                </li>
                <li v-for="n in totalPages" :key="n" class="page-item" :class="{ active: n === currentPage }">
                <button class="page-link" @click="handlePageChange(n)">{{ n }}</button>
                </li>
                <li class="page-item" :class="{ disabled: currentPage === totalPages }">
                <button class="page-link" @click="handlePageChange(currentPage + 1)">Next</button>
                </li>
            </ul>
        </nav>
    </div>
</template>