<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import { closeModal  } from '../../services/modal/ModalFunction';
import { AccountService } from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import ClubOfferService from '../../services/api/ClubOfferService';
import ChatService from '../../services/api/ChatService';
import { TimeService } from '../../services/time/TimeService';
import type { ClubOffer } from '../../models/interfaces/ClubOffer';
import type { ChatCreateDTO } from '../../models/dtos/ChatCreateDTO';
import '../../styles/user/MyOffersAsClub.css';

const toast = useToast();
const router = useRouter();
const route = useRoute();

const userId = ref<string | null>(null);
const sentClubOffers = ref<ClubOffer[]>([]);
const selectedClubOffer = ref<ClubOffer | null>(null);
const deleteClubOfferId = ref<number | null>(null);

onMounted(async () => {
  if (route.query.toastMessage) {
    toast.success(route.query.toastMessage as string);
  }

  try {
    const id = await AccountService.getId();
    if (id) {
      userId.value = id;
      sentClubOffers.value = await UserService.getSentClubOffers(id);
    }
  }
  catch (error) {
    console.error('Failed to fetch user data:', error);
    toast.error('Failed to load user data.');
  }
});

const handleShowClubOfferDetails = (clubOffer: ClubOffer) => {
    selectedClubOffer.value = clubOffer;
    deleteClubOfferId.value = clubOffer.id;
};

const moveToPlayerAdvertisementPage = (playerAdvertisementId: number) => {
  router.push({ path: `/player-advertisement/${playerAdvertisementId}` });
};

const handleDeleteClubOffer = async () => {
  if (!deleteClubOfferId.value)
    return;

  try {
    await ClubOfferService.deleteClubOffer(deleteClubOfferId.value);
    toast.success('Your sent club offer has been deleted successfully.');
    deleteClubOfferId.value = null;

    if (userId.value) {
      sentClubOffers.value = await UserService.getSentClubOffers(userId.value);
    }
    closeModal('deleteOfferModal');
    closeModal('offerDetailsModal');
  }
  catch (error) {
    console.error('Failed to delete sent club offer:', error);
    toast.error('Failed to delete sent club offer.');
  }
};

const handleOpenChat = async (receiverId: string) => {
  if (!receiverId || !userId.value)
    return;

  try {
    let chatId = await ChatService.getChatIdBetweenUsers(userId.value, receiverId);

    if (chatId === 0) {
      const chatCreateDTO: ChatCreateDTO = {
        user1Id: userId.value,
        user2Id: receiverId,
      };

      await ChatService.createChat(chatCreateDTO);
      chatId = await ChatService.getChatIdBetweenUsers(userId.value, receiverId);
    }
    router.push({ path: `/chat/${chatId}` });
  }
  catch (error) {
    console.error('Failed to open chat:', error);
    toast.error('Failed to open chat.');
  }
};
</script>

<template>
  <div class="MyOffersAsClub">
    <h1><i class="bi bi-briefcase-fill"></i> My Offers as a Club member</h1>
    
    <h3><i class="bi bi-send-fill"></i> My sent offers to players</h3>
    <div class="table-responsive">
      <table class="table table-striped table-bordered table-hover">
        <thead class="table-success">
          <tr>
            <th>Sent Date</th>
            <th>Offer Status</th>
            <th>Player</th>
            <th>Position</th>
            <th>Club Name</th>
            <th>League (Region)</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="clubOffer in sentClubOffers" :key="clubOffer.id">
            <td>{{ TimeService.formatDateToEUR(clubOffer.creationDate) }}</td>
            <td>
              <i v-if="clubOffer.offerStatus.statusName === 'Offered'" class="bi bi-question-diamond-fill" style="color: #b571ff"></i>
              <i v-else-if="clubOffer.offerStatus.statusName === 'Accepted'" class="bi bi-check-circle-fill" style="color: green"></i>
              <i v-else-if="clubOffer.offerStatus.statusName === 'Rejected'" class="bi bi-x-circle-fill" style="color: red"></i>
              {{ clubOffer.offerStatus.statusName }}
            </td>
            <td>{{ clubOffer.playerAdvertisement.player.firstName }} {{ clubOffer.playerAdvertisement.player.lastName }}</td>
            <td>{{ clubOffer.playerPosition.positionName }}</td>
            <td>{{ clubOffer.clubName }}</td>
            <td>{{ clubOffer.league }} ({{ clubOffer.region }})</td>
            <td>
              <button class="btn btn-primary btn-sm me-2" data-bs-toggle="modal" data-bs-target="#offerDetailsModal" @click="handleShowClubOfferDetails(clubOffer)">
                <i class="bi bi-info-circle"></i> Offer
              </button>
              <button class="btn btn-dark btn-sm me-2" @click="moveToPlayerAdvertisementPage(clubOffer.playerAdvertisement.id)">
                <i class="bi bi-info-square"></i> Ad
              </button>
              <button class="btn btn-info btn-sm" @click="handleOpenChat(clubOffer.playerAdvertisement.playerId)">
                <i class="bi bi-chat-fill"></i>
              </button>
            </td>
          </tr>
          <tr v-if="sentClubOffers.length === 0">
            <td colspan="7" class="text-center">No sent club offer available</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Offer Details Modal -->
    <div class="modal" id="offerDetailsModal" tabindex="-1">
        <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Offer Details</h5>
                <button type="button" class="btn-close" @click="closeModal('offerDetailsModal')"></button>
            </div>
            <div class="modal-body" v-if="selectedClubOffer">
            <div class="text-center">
                <button class="btn btn-danger mb-3" data-bs-toggle="modal" data-bs-target="#deleteOfferModal" >
                <i class="bi bi-trash"></i> Delete
                </button>
            </div>
            <p><strong class="offer-name-label">{{ selectedClubOffer.clubName }}</strong></p>
            <p><strong class="offer-position-label">{{ selectedClubOffer.playerPosition.positionName }}</strong></p>

            <h5 class="offer-section">OFFER INFO</h5>
            <p><strong>Sent Date:</strong> {{ TimeService.formatDateToEUR(selectedClubOffer.creationDate) }}</p>
            <p><strong>End Date (days left/passed):</strong> 
                {{ TimeService.formatDateToEUR(selectedClubOffer.playerAdvertisement.endDate) }} 
                ({{ TimeService.calculateDaysLeftPassed(selectedClubOffer.playerAdvertisement.endDate) }})
            </p>
            <p>
                <strong>Offer status: </strong>
                <i v-if="selectedClubOffer.offerStatus.statusName === 'Offered'" class="bi bi-question-diamond-fill text-purple"></i>
                <i v-else-if="selectedClubOffer.offerStatus.statusName === 'Accepted'" class="bi bi-check-circle-fill text-success"></i>
                <i v-else-if="selectedClubOffer.offerStatus.statusName === 'Rejected'" class="bi bi-x-circle-fill text-danger"></i>
                {{ selectedClubOffer.offerStatus.statusName }}
            </p>

            <h5 class="offer-section">SENT TO</h5>
            <p><strong>Name:</strong> {{ selectedClubOffer.playerAdvertisement.player.firstName }} {{ selectedClubOffer.playerAdvertisement.player.lastName }}</p>
            <p><strong>E-mail:</strong> {{ selectedClubOffer.playerAdvertisement.player.email }}</p>
            <p><strong>Phone number:</strong> {{ selectedClubOffer.playerAdvertisement.player.phoneNumber }}</p>

            <h5 class="offer-section">DETAILS</h5>
            <p><strong>Club Name:</strong> {{ selectedClubOffer.clubName }}</p>
            <p><strong>League:</strong> {{ selectedClubOffer.league }}</p>
            <p><strong>Region:</strong> {{ selectedClubOffer.region }}</p>
            <p><strong>Position:</strong> {{ selectedClubOffer.playerPosition.positionName }}</p>
            <p><strong>Salary (zł.) / month:</strong> {{ selectedClubOffer.salary }}</p>
            <p><strong>Additional Information:</strong> {{ selectedClubOffer.additionalInformation }}</p>
            </div>
            <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeModal('offerDetailsModal')">Close</button>
            </div>
        </div>
        </div>
    </div>

    <!-- Delete Offer Modal -->
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
                <button class="btn btn-danger" @click="handleDeleteClubOffer">Delete</button>
                </div>
            </div>
        </div>
    </div>
  </div>
</template>