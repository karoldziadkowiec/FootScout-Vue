<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import { closeModal  } from '../../services/modal/ModalFunction';
import { AccountService } from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import { TimeService } from '../../services/time/TimeService';
import FavoritePlayerAdvertisementService from '../../services/api/FavoritePlayerAdvertisementService';
import ChatService from '../../services/api/ChatService';
import type { FavoritePlayerAdvertisement } from '../../models/interfaces/FavoritePlayerAdvertisement';
import type { ChatCreateDTO } from '../../models/dtos/ChatCreateDTO';
import '../../styles/user/MyFavoritePlayerAdvertisements.css';

const toast = useToast();
const router = useRouter();
const route = useRoute();

const userId = ref<string | null>(null);
const userActiveFavoritePlayerAdvertisements = ref<FavoritePlayerAdvertisement[]>([]);
const userInactiveFavoritePlayerAdvertisements = ref<FavoritePlayerAdvertisement[]>([]);
const deleteFavoriteId = ref<number | null>(null);

onMounted(async () => {
    if (route.query.toastMessage) {
        toast.success(route.query.toastMessage as string);
    }
    await fetchUserData();
});

const fetchUserData = async () => {
    try {
        const id = await AccountService.getId();
        if (id) {
            userId.value = id;
            userActiveFavoritePlayerAdvertisements.value = await UserService.getUserActivePlayerAdvertisementFavorites(id);
            userInactiveFavoritePlayerAdvertisements.value = await UserService.getUserInactivePlayerAdvertisementFavorites(id);
        }
    }
    catch (error) {
        console.error("Failed to fetch user's data:", error);
        toast.error("Failed to load user's data.");
    }
};

const moveToPlayerAdvertisementPage = (playerAdvertisementId: number) => {
    router.push({ path: `/player-advertisement/${playerAdvertisementId}`, state: { playerAdvertisementId }, });
};

const handleShowDeleteModal = (favoriteAdvertisementId: number) => {
    deleteFavoriteId.value = favoriteAdvertisementId;
};

const handleDeleteFromFavorites = async () => {
    if (!userId.value || !deleteFavoriteId.value) return;

    try {
        await FavoritePlayerAdvertisementService.deleteFromFavorites(deleteFavoriteId.value);
        toast.success("Your followed advertisement has been deleted from favorites successfully.");
        deleteFavoriteId.value = null;
        await fetchUserData();
        closeModal('deleteFromFavoritesModal');
    }
    catch (error) {
        console.error("Failed to delete advertisement from favorites:", error);
        toast.error("Failed to delete advertisement from favorites.");
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
                user2Id: receiverId
            };

            await ChatService.createChat(chatCreateDTO);
            chatId = await ChatService.getChatIdBetweenUsers(userId.value, receiverId);
        }
        router.push({ path: `/chat/${chatId}`, state: { chatId } });
    }
    catch (error) {
        console.error("Failed to open chat:", error);
        toast.error("Failed to open chat.");
    }
};
</script>

<template>
    <div class="MyFavoritePlayerAdvertisements">
        <h1><i class="bi bi-chat-square-heart"></i> My Favorite Player Advertisements</h1>

        <!-- Zakładki -->
        <ul class="nav nav-tabs mb-3" id="myTab" role="tablist">
            <li class="nav-item" role="presentation">
                <button class="nav-link active" id="active-tab" data-bs-toggle="tab" data-bs-target="#active" type="button" role="tab">
                    Active advertisements
                </button>
            </li>
            <li class="nav-item" role="presentation">
                <button class="nav-link" id="archived-tab" data-bs-toggle="tab" data-bs-target="#archived" type="button" role="tab">
                    Archived advertisements
                </button>
            </li>
        </ul>

        <div class="tab-content">
            <!-- Active advertisements -->
            <div class="tab-pane fade show active" id="active" role="tabpanel">
                <h3><i class="bi bi-bookmark-check"></i> Active advertisements</h3>
                <div class="table-responsive">
                    <table class="table table-striped table-bordered table-hover">
                        <thead class="table-success">
                            <tr>
                                <th>End Date (days left)</th>
                                <th>Player</th>
                                <th>Position</th>
                                <th>Preferred League (Region)</th>
                                <th>Salary (zł.) / month</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-if="userActiveFavoritePlayerAdvertisements.length === 0">
                                <td colspan="6" class="text-center">No favorite player advertisement available</td>
                            </tr>
                            <tr v-for="(favoriteAdvertisement, index) in userActiveFavoritePlayerAdvertisements" :key="index">
                                <td>
                                    {{ TimeService.formatDateToEUR(favoriteAdvertisement.playerAdvertisement.endDate) }}
                                    ({{ TimeService.calculateDaysLeft(favoriteAdvertisement.playerAdvertisement.endDate) }} days)
                                </td>
                                <td>
                                    {{ favoriteAdvertisement.playerAdvertisement.player.firstName }}
                                    {{ favoriteAdvertisement.playerAdvertisement.player.lastName }}
                                </td>
                                <td>{{ favoriteAdvertisement.playerAdvertisement.playerPosition.positionName }}</td>
                                <td>
                                    {{ favoriteAdvertisement.playerAdvertisement.league }} 
                                    ({{ favoriteAdvertisement.playerAdvertisement.region }})
                                </td>
                                <td>
                                    {{ favoriteAdvertisement.playerAdvertisement.salaryRange.min }} - 
                                    {{ favoriteAdvertisement.playerAdvertisement.salaryRange.max }}
                                </td>
                                <td>
                                    <button class="btn btn-dark me-1" @click="moveToPlayerAdvertisementPage(favoriteAdvertisement.playerAdvertisement.id)">
                                        <i class="bi bi-info-square"></i>
                                    </button>
                                    <button class="btn btn-danger me-1" data-bs-toggle="modal" data-bs-target="#deleteFromFavoritesModal" @click="handleShowDeleteModal(favoriteAdvertisement.id)">
                                        <i class="bi bi-heart-fill"></i>
                                    </button>
                                    <button class="btn btn-info"@click="handleOpenChat(favoriteAdvertisement.playerAdvertisement.playerId)">
                                        <i class="bi bi-chat-fill"></i>
                                    </button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>

            <!-- Archived advertisements -->
            <div class="tab-pane fade" id="archived" role="tabpanel">
                <h3><i class="bi bi-clipboard-x"></i> Archived advertisements</h3>
                <div class="table-responsive">
                    <table class="table table-striped table-bordered table-hover">
                        <thead class="table-warning">
                            <tr>
                                <th>End Date (days passed)</th>
                                <th>Player</th>
                                <th>Position</th>
                                <th>Preferred League (Region)</th>
                                <th>Salary (zł.) / month</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-if="userInactiveFavoritePlayerAdvertisements.length === 0">
                                <td colspan="6" class="text-center">No favorite player advertisement available</td>
                            </tr>
                            <tr v-for="(favoriteAdvertisement, index) in userInactiveFavoritePlayerAdvertisements" :key="index">
                                <td>
                                    {{ TimeService.formatDateToEUR(favoriteAdvertisement.playerAdvertisement.endDate) }}
                                    ({{ TimeService.calculateSkippedDays(favoriteAdvertisement.playerAdvertisement.endDate) }} days)
                                </td>
                                <td>
                                    {{ favoriteAdvertisement.playerAdvertisement.player.firstName }}
                                    {{ favoriteAdvertisement.playerAdvertisement.player.lastName }}
                                </td>
                                <td>{{ favoriteAdvertisement.playerAdvertisement.playerPosition.positionName }}</td>
                                <td>
                                    {{ favoriteAdvertisement.playerAdvertisement.league }} 
                                    ({{ favoriteAdvertisement.playerAdvertisement.region }})
                                </td>
                                <td>
                                    {{ favoriteAdvertisement.playerAdvertisement.salaryRange.min }} - 
                                    {{ favoriteAdvertisement.playerAdvertisement.salaryRange.max }}
                                </td>
                                <td>
                                    <button class="btn btn-dark me-1" @click="moveToPlayerAdvertisementPage(favoriteAdvertisement.playerAdvertisement.id)">
                                        <i class="bi bi-info-square"></i>
                                    </button>
                                    <button class="btn btn-danger" @click="handleShowDeleteModal(favoriteAdvertisement.id)">
                                        <i class="bi bi-heart-fill"></i>
                                    </button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

        <!-- Modal usuwania ogłoszenia z ulubionych -->
        <div class="modal" id="deleteFromFavoritesModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="deleteModalLabel">Confirm action</h5>
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
    </div>
</template>