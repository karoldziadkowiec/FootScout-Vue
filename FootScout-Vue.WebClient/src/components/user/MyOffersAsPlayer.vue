<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import { closeModal  } from '../../services/modal/ModalFunction';
import { AccountService } from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import ClubOfferService from '../../services/api/ClubOfferService';
import { TimeService } from '../../services/time/TimeService';
import ChatService from '../../services/api/ChatService';
import { OfferStatusName } from '../../models/enums/OfferStatusName';
import type { ClubOffer } from '../../models/interfaces/ClubOffer';
import type { ChatCreateDTO } from '../../models/dtos/ChatCreateDTO';
import '../../styles/user/MyOffersAsPlayer.css';

// MyOffersAsPlayer.vue - Komponent zarządzający ofertami klubów piłkarskich dla gracza

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const route = useRoute();   // Pobranie informacji o aktualnej trasie (np. parametry w URL)
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Deklaracja zmiennych reaktywnych dla danych użytkownika i ofert klubów
const userId = ref<string | null>(null);
const receivedClubOffers = ref<ClubOffer[]>([]);

// Zmienna przechowująca wybraną ofertę klubu oraz oferty do akceptacji i odrzucenia
const selectedReceivedClubOffer = ref<ClubOffer | null>(null);
const receivedClubOfferToAccept = ref<ClubOffer | null>(null);
const receivedClubOfferToReject = ref<ClubOffer | null>(null);

// Funkcja wywoływana po załadowaniu komponentu, ładująca dane użytkownika i oferty klubów
onMounted(async () => {
    if (route.query.toastMessage) {
        toast.success(route.query.toastMessage as string);
    }

    try {
        // Próba pobrania identyfikatora użytkownika i ofert klubów
        const id = await AccountService.getId();
        if (id) {
            userId.value = id;
            receivedClubOffers.value = await UserService.getReceivedClubOffers(id);     // Pobranie ofert klubów
        }
    }
    catch (error) {
        console.error('Failed to fetch user data:', error);
        toast.error('Failed to load user data.');
    }
});

// Funkcja obsługująca wyświetlanie szczegółów wybranej oferty klubu
const handleShowReceivedClubOfferDetails = (clubOffer: ClubOffer) => {
    selectedReceivedClubOffer.value = clubOffer;
};

// Funkcja wyświetlająca modal do akceptacji oferty
const handleShowAcceptReceivedClubOfferModal = (clubOffer: ClubOffer) => {
    receivedClubOfferToAccept.value = clubOffer;
};

// Funkcja akceptująca ofertę klubu
const acceptReceivedClubOffer = async () => {
    if (!receivedClubOfferToAccept.value || !userId.value)      // Sprawdzenie, czy istnieje oferta i użytkownik
        return;

    try {
        // Wywołanie usługi do akceptacji oferty
        await ClubOfferService.acceptClubOffer(receivedClubOfferToAccept.value.id, receivedClubOfferToAccept.value);
        toast.success('Received club offer has been accepted successfully.');
        // Odświeżenie listy ofert po akceptacji
        receivedClubOffers.value = await UserService.getReceivedClubOffers(userId.value);
        closeModal('acceptOfferModal');
    }
    catch (error) {
        console.error('Failed to accept received club offer:', error);
        toast.error('Failed to accept received club offer.');
    }
};

// Funkcja wyświetlająca modal do odrzucenia oferty
const handleShowRejectReceivedClubOfferModal = (clubOffer: ClubOffer) => {
    receivedClubOfferToReject.value = clubOffer;
};

// Funkcja odrzucająca ofertę klubu
const rejectReceivedClubOffer = async () => {
    if (!receivedClubOfferToReject.value || !userId.value)
        return;

    try {
        await ClubOfferService.rejectClubOffer(receivedClubOfferToReject.value.id, receivedClubOfferToReject.value);
        toast.success('Received club offer has been rejected successfully.');
        // Odświeżenie listy ofert po odrzuceniu
        receivedClubOffers.value = await UserService.getReceivedClubOffers(userId.value);
        closeModal('rejectOfferModal');
    }
    catch (error) {
        console.error('Failed to reject received club offer:', error);
        toast.error('Failed to reject received club offer.');
    }
};

// Funkcja przechodząca do strony ogłoszenia piłkarza
const moveToPlayerAdvertisementPage = (playerAdvertisementId: number) => {
    router.push({ path: `/player-advertisement/${playerAdvertisementId}`, state: { playerAdvertisementId } });
};

// Funkcja otwierająca czat z innym użytkownikiem
const handleOpenChat = async (receiverId: string) => {
    if (!receiverId || !userId.value)
        return;                 // Sprawdzenie, czy istnieją obydwa identyfikatory

    try {
        // Próba pobrania identyfikatora czatu pomiędzy użytkownikami
        let chatId = await ChatService.getChatIdBetweenUsers(userId.value, receiverId);

        // Jeśli czat nie istnieje, tworzony jest nowy
        if (chatId === 0) {
            const chatCreateDTO: ChatCreateDTO = {
                user1Id: userId.value,
                user2Id: receiverId
            };

            await ChatService.createChat(chatCreateDTO);
            chatId = await ChatService.getChatIdBetweenUsers(userId.value, receiverId);
        }
        router.push({ path: `/chat/${chatId}`, state: { chatId } });        // Przejście do strony czatu
    }
    catch (error) {
        console.error('Failed to open chat:', error);
        toast.error('Failed to open chat.');
    }
};

</script>
<!-- Struktura strony gracza: lista ofert klubów oraz opcje akceptacji/odrzucenia -->
<template>
    <div class="MyOffersAsPlayer">
        <h1><i class="bi bi-briefcase"></i> My Offers as a Player</h1>
        <h3><i class="bi bi-arrow-down-left-square"></i> Received offers from clubs</h3>
        <div class="table-responsive">
            <table class="table table-striped table-bordered table-hover table-light">
                <thead class="table-dark">
                <tr>
                    <th>Received Date</th>
                    <th>Offer Status</th>
                    <th>Club Name</th>
                    <th>League (Region)</th>
                    <th>Position</th>
                    <th></th>
                </tr>
                </thead>
                <tbody>
                <template v-if="receivedClubOffers.length > 0">
                    <tr v-for="(clubOffer, index) in receivedClubOffers" :key="index">
                    <td class="text-center">{{ TimeService.formatDateToEUR(clubOffer.creationDate) }}</td>
                    <td class="text-center">
                        <i v-if="clubOffer.offerStatus.statusName === 'Offered'" class="bi bi-question-diamond-fill text-purple"></i>
                        <i v-if="clubOffer.offerStatus.statusName === 'Accepted'" class="bi bi-check-circle-fill text-success"></i>
                        <i v-if="clubOffer.offerStatus.statusName === 'Rejected'" class="bi bi-x-circle-fill text-danger"></i>
                        {{ clubOffer.offerStatus.statusName }}
                    </td>
                    <td class="text-center">{{ clubOffer.clubName }}</td>
                    <td class="text-center">{{ clubOffer.league }} ({{ clubOffer.region }})</td>
                    <td class="text-center">{{ clubOffer.playerPosition.positionName }}</td>
                    <td class="text-center">
                        <button class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#offerDetailsModal" @click="handleShowReceivedClubOfferDetails(clubOffer)">
                            <i class="bi bi-info-circle"></i> Offer
                        </button>
                        
                        <template v-if="clubOffer.offerStatus.statusName === OfferStatusName.Offered && new Date(clubOffer.playerAdvertisement.endDate) > new Date()">
                        <button class="btn btn-success btn-sm me-2" data-bs-toggle="modal" data-bs-target="#acceptOfferModal" @click="handleShowAcceptReceivedClubOfferModal(clubOffer)">
                            <i class="bi bi-check-lg"></i>
                        </button>
                        <button class="btn btn-danger btn-sm me-2" data-bs-toggle="modal" data-bs-target="#rejectOfferModal" @click="handleShowRejectReceivedClubOfferModal(clubOffer)">
                            <i class="bi bi-x"></i>
                        </button>
                        </template>
                        
                        <span>| </span>
                        
                        <button class="btn btn-dark btn-sm me-2" @click="moveToPlayerAdvertisementPage(clubOffer.playerAdvertisement.id)">
                        <i class="bi bi-info-square"></i> Ad
                        </button>
                        
                        <button class="btn btn-info btn-sm text-white" @click="handleOpenChat(clubOffer.clubMemberId)">
                        <i class="bi bi-chat-fill"></i>
                        </button>
                    </td>
                    </tr>
                </template>
                <tr v-else>
                    <td colspan="6" class="text-center">No received club offer available</td>
                </tr>
                </tbody>
            </table>
        </div>
        
        <!-- Modal ze szczegółami oferty klubu -->
        <div class="modal" id="offerDetailsModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Offer Details</h5>
                        <button type="button" class="btn-close" @click="closeModal('offerDetailsModal')"></button>
                    </div>
                    <div class="modal-body" v-if="selectedReceivedClubOffer">
                        <div class="modal-content-centered">
                            <p><strong class="offer-name-label">{{ selectedReceivedClubOffer.clubName.toUpperCase() }}</strong></p>
                            <p><strong class="offer-position-label">{{ selectedReceivedClubOffer.playerPosition.positionName }}</strong></p>

                            <h6 class="offer-section">OFFER INFO</h6>
                            <p><strong>Received Date:</strong> {{ TimeService.formatDateToEUR(selectedReceivedClubOffer.creationDate) }}</p>
                            <p><strong>End Date (days left/passed):</strong> 
                                {{ TimeService.formatDateToEUR(selectedReceivedClubOffer.playerAdvertisement.endDate) }} 
                                ({{ TimeService.calculateDaysLeftPassed(selectedReceivedClubOffer.playerAdvertisement.endDate) }})
                            </p>

                            <p>
                                <strong>Offer status: </strong>
                                <i v-if="selectedReceivedClubOffer.offerStatus.statusName === 'Offered'" class="bi bi-question-diamond-fill text-purple"></i>
                                <i v-if="selectedReceivedClubOffer.offerStatus.statusName === 'Accepted'" class="bi bi-check-circle-fill text-success"></i>
                                <i v-if="selectedReceivedClubOffer.offerStatus.statusName === 'Rejected'" class="bi bi-x-circle-fill text-danger"></i>
                                {{ selectedReceivedClubOffer.offerStatus.statusName }}
                            </p>

                            <h6 class="offer-section">DETAILS</h6>
                            <p><strong>Club Name:</strong> {{ selectedReceivedClubOffer.clubName }}</p>
                            <p><strong>League:</strong> {{ selectedReceivedClubOffer.league }}</p>
                            <p><strong>Region:</strong> {{ selectedReceivedClubOffer.region }}</p>
                            <p><strong>Position:</strong> {{ selectedReceivedClubOffer.playerPosition.positionName }}</p>
                            <p><strong>Salary (zł.) / month:</strong> {{ selectedReceivedClubOffer.salary }}</p>
                            <p><strong>Additional Information:</strong> {{ selectedReceivedClubOffer.additionalInformation }}</p>

                            <h6 class="offer-section">RECEIVED FROM</h6>
                            <p><strong>Name:</strong> {{ selectedReceivedClubOffer.clubMember.firstName }} {{ selectedReceivedClubOffer.clubMember.lastName }}</p>
                            <p><strong>E-mail:</strong> {{ selectedReceivedClubOffer.clubMember.email }}</p>
                            <p><strong>Phone number:</strong> {{ selectedReceivedClubOffer.clubMember.phoneNumber }}</p>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" @click="closeModal('offerDetailsModal')">Close</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal akceptowania oferty klubu  -->
        <div class="modal" id="acceptOfferModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Confirm action</h5>
                        <button type="button" class="btn-close" @click="closeModal('acceptOfferModal')"></button>
                    </div>
                    <div class="modal-body">
                        Are you sure you want to accept this club offer?
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" @click="closeModal('acceptOfferModal')">Cancel</button>
                        <button type="button" class="btn btn-success" @click="acceptReceivedClubOffer">Accept</button>
                    </div>
                </div>
            </div>
        </div>
        
        <!-- Modal odrzucania oferty klubu -->
        <div class="modal" id="rejectOfferModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Confirm action</h5>
                        <button type="button" class="btn-close" @click="closeModal('rejectOfferModal')"></button>
                    </div>
                    <div class="modal-body">
                        Are you sure you want to reject this club offer?
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" @click="closeModal('rejectOfferModal')">Cancel</button>
                        <button type="button" class="btn btn-danger" @click="rejectReceivedClubOffer">Reject</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>