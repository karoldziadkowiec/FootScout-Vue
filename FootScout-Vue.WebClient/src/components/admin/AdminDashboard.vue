<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import CurrentTimeDisplay from '../../services/time/CurrentTimeDisplay.vue';
import UserService from '../../services/api/UserService';
import ChatService from '../../services/api/ChatService';
import ProblemService from '../../services/api/ProblemService';
import PlayerAdvertisementService from '../../services/api/PlayerAdvertisementService';
import ClubAdvertisementService from '../../services/api/ClubAdvertisementService';
import PlayerOfferService from '../../services/api/PlayerOfferService';
import ClubOfferService from '../../services/api/ClubOfferService';
import '../../styles/admin/AdminDashboard.css';

const router = useRouter();
const route = useRoute();
const toast = useToast();

const userCount = ref<number>(0);
const chatCount = ref<number>(0);
const unsolvedReportedProblemCount = ref<number>(0);
const playerAdvertisementCount = ref<number>(0);
const clubAdvertisementCount = ref<number>(0);
const playerOfferCount = ref<number>(0);
const clubOfferCount = ref<number>(0);

const fetchCountData = async () => {
  try {
    userCount.value = await UserService.getUserCount();
    chatCount.value = await ChatService.getChatCount();
    unsolvedReportedProblemCount.value = await ProblemService.getUnsolvedProblemCount();
    playerAdvertisementCount.value = await PlayerAdvertisementService.getActivePlayerAdvertisementCount();
    clubAdvertisementCount.value = await ClubAdvertisementService.getActiveClubAdvertisementCount();
    playerOfferCount.value = await PlayerOfferService.getActivePlayerOfferCount();
    clubOfferCount.value = await ClubOfferService.getActiveClubOfferCount();
  } 
  catch (error) {
    console.error('Failed to fetch count data:', error);
  }
};

onMounted(() => {
  if (route.query.toastMessage) {
    toast.success(route.query.toastMessage as string);
  }
  fetchCountData();
});

const handleCardClick = (route: string) => {
  router.push(route);
};
</script>

<template>
  <div class="AdminDashboard">
    <h1><i class="bi bi-grid"></i> Dashboard</h1>
    <CurrentTimeDisplay />
    
    <button class="btn btn-dark mt-3" @click="fetchCountData">
      <i class="bi bi-arrow-repeat"></i> Refresh
    </button>

    <div class="row mt-4">
      <div class="col-md-4 mb-4">
        <div class="card bg-primary text-white clickable-card" @click="handleCardClick('/admin/users')">
          <div class="card-body">
            <h5 class="card-title"><i class="bi bi-people-fill"></i> Users</h5>
            <h4>{{ userCount }}</h4>
          </div>
        </div>
      </div>
      <div class="col-md-4 mb-4">
        <div class="card bg-info text-white clickable-card" @click="handleCardClick('/admin/chats')">
          <div class="card-body">
            <h5 class="card-title"><i class="bi bi-chat-text-fill"></i> Chats</h5>
            <h4>{{ chatCount }}</h4>
          </div>
        </div>
      </div>
      <div class="col-md-4 mb-4">
        <div class="card bg-danger text-white clickable-card" @click="handleCardClick('/admin/support')">
          <div class="card-body">
            <h5 class="card-title"><i class="bi bi-cone-striped"></i> Reported Problems</h5>
            <h4>{{ unsolvedReportedProblemCount }}</h4>
          </div>
        </div>
      </div>
    </div>

    <div class="row">
      <div class="col-md-3 mb-4">
        <div class="card bg-success text-white clickable-card" @click="handleCardClick('/admin/player-advertisements')">
          <div class="card-body">
            <h5 class="card-title"><i class="bi bi-person-bounding-box"></i> Player Ads</h5>
            <h4>{{ playerAdvertisementCount }}</h4>
          </div>
        </div>
      </div>
      <div class="col-md-3 mb-4">
        <div class="card bg-warning text-white clickable-card" @click="handleCardClick('/admin/club-advertisements')">
          <div class="card-body">
            <h5 class="card-title"><i class="bi bi-shield-fill"></i> Club Ads</h5>
            <h4>{{ clubAdvertisementCount }}</h4>
          </div>
        </div>
      </div>
      <div class="col-md-3 mb-4">
        <div class="card bg-dark text-white clickable-card" @click="handleCardClick('/admin/player-offers')">
          <div class="card-body">
            <h5 class="card-title"><i class="bi bi-briefcase"></i> Player Offers</h5>
            <h4>{{ playerOfferCount }}</h4>
          </div>
        </div>
      </div>
      <div class="col-md-3 mb-4">
        <div class="card bg-secondary text-white clickable-card" @click="handleCardClick('/admin/club-offers')">
          <div class="card-body">
            <h5 class="card-title"><i class="bi bi-briefcase-fill"></i> Club Offers</h5>
            <h4>{{ clubOfferCount }}</h4>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>