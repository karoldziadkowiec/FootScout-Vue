<script setup lang="ts">
import { ref, onMounted, watchEffect, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import { closeModal  } from '../../services/modal/ModalFunction';
import { TimeService } from '../../services/time/TimeService';
import { AccountService } from '../../services/api/AccountService';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import PlayerAdvertisementService from '../../services/api/PlayerAdvertisementService';
import ChatService from '../../services/api/ChatService';
import type { PlayerPosition } from '../../models/interfaces/PlayerPosition';
import type { PlayerAdvertisement } from '../../models/interfaces/PlayerAdvertisement';
import type { ChatCreateDTO } from '../../models/dtos/ChatCreateDTO';
import '../../styles/admin/AdminPlayerAdvertisements.css';

const toast = useToast();
const router = useRouter();
const route = useRoute();

const userId = ref<string | null>(null);
const positions = ref<PlayerPosition[]>([]);
const playerAdvertisements = ref<PlayerAdvertisement[]>([]);
const finishAdvertisementId = ref<number | null>(null);
const deleteAdvertisementId = ref<number | null>(null);

// Wyszukiwanie i sortowanie
const searchTerm = ref<string>('');
const selectedStatus = ref<'all' | 'active' | 'inactive'>('all');
const selectedPosition = ref<string | ''>('');
const sortCriteria = ref<string>('creationDateDesc');
// Paginacja
const currentPage = ref<number>(1);
const itemsPerPage = 20;

onMounted(async () => {
    try {
        userId.value = await AccountService.getId();
        positions.value = await PlayerPositionService.getPlayerPositions();
        playerAdvertisements.value = await PlayerAdvertisementService.getAllPlayerAdvertisements();
    }
    catch (error) {
        console.error('Failed to fetch data:', error);
        toast.error('Failed to load data.');
    }
});

watchEffect(async () => {
    if (route.fullPath) {
        try {
            playerAdvertisements.value = await PlayerAdvertisementService.getAllPlayerAdvertisements();
        }
        catch (error) {
            console.error('Failed to fetch player advertisements:', error);
            toast.error('Failed to load player advertisements.');
        }
    }
});

const handlePageChange = (pageNumber: number) => {
    currentPage.value = pageNumber;
};

const moveToPlayerAdvertisementPage = (playerAdvertisementId: number) => {
    router.push({ path: `/player-advertisement/${playerAdvertisementId}`, state: { playerAdvertisementId } });
};

const handleShowFinishModal = (advertisementId: number) => {
    finishAdvertisementId.value = advertisementId;
};

const finishPlayerAdvertisement = async () => {
    if (!finishAdvertisementId.value)
        return;

    try {
        const playerAdvertisement = await PlayerAdvertisementService.getPlayerAdvertisement(finishAdvertisementId.value);
        const currentDate = new Date().toISOString();

        const finishFormData = {
            ...playerAdvertisement,
            endDate: currentDate
        };

        await PlayerAdvertisementService.updatePlayerAdvertisement(finishAdvertisementId.value, finishFormData);
        toast.success('Advertisement has been finished successfully.');

        playerAdvertisements.value = await PlayerAdvertisementService.getAllPlayerAdvertisements();
        closeModal('finishAdvertisementModal');
    }
    catch (error) {
        console.error('Failed to finish advertisement:', error);
        toast.error('Failed to finish advertisement.');
    }
};

const handleShowDeleteModal = (advertisementId: number) => {
    deleteAdvertisementId.value = advertisementId;
};

const deleteAdvertisement = async () => {
    if (!deleteAdvertisementId.value)
        return;

    try {
        await PlayerAdvertisementService.deletePlayerAdvertisement(deleteAdvertisementId.value);
        toast.success('Advertisement has been deleted successfully.');

        playerAdvertisements.value = await PlayerAdvertisementService.getAllPlayerAdvertisements();
        closeModal('deleteAdvertisementModal');
    }
    catch (error) {
        console.error('Failed to delete advertisement:', error);
        toast.error('Failed to delete advertisement.');
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
        console.error('Failed to open chat:', error);
        toast.error('Failed to open chat.');
    }
};

const filteredAdvertisements = computed(() => {
    let ads = playerAdvertisements.value;

    if (searchTerm.value) {
        const lowerCaseSearchTerm = searchTerm.value.toLowerCase();
        ads = ads.filter(ad =>
            `${ad.player.firstName} ${ad.player.lastName}`.toLowerCase().includes(lowerCaseSearchTerm) ||
            ad.league.toLowerCase().includes(lowerCaseSearchTerm) ||
            ad.region.toLowerCase().includes(lowerCaseSearchTerm)
        );
    }

    if (selectedStatus.value !== 'all') {
        const now = Date.now();
        ads = ads.filter(ad => 
            selectedStatus.value === 'active'
                ? new Date(ad.endDate).getTime() >= now
                : new Date(ad.endDate).getTime() < now
        );
    }

    if (selectedPosition.value) {
        ads = ads.filter(ad => ad.playerPosition.id === parseInt(selectedPosition.value, 10));
    }

    return ads.sort((a, b) => {
        switch (sortCriteria.value) {
            case 'creationDateAsc': return new Date(a.creationDate).getTime() - new Date(b.creationDate).getTime();
            case 'creationDateDesc': return new Date(b.creationDate).getTime() - new Date(a.creationDate).getTime();
            case 'positionAsc': return a.playerPosition.positionName.localeCompare(b.playerPosition.positionName);
            case 'positionDesc': return b.playerPosition.positionName.localeCompare(a.playerPosition.positionName);
            case 'leagueAsc': return a.league.localeCompare(b.league);
            case 'leagueDesc': return b.league.localeCompare(a.league);
            case 'regionAsc': return a.region.localeCompare(b.region);
            case 'regionDesc': return b.region.localeCompare(a.region);
            case 'salaryAsc': return a.salaryRange.min - b.salaryRange.min;
            case 'salaryDesc': return b.salaryRange.min - a.salaryRange.min;
            default: return 0;
        }
    });
});

const currentPlayerAdvertisementItems = computed(() => {
    const start = (currentPage.value - 1) * itemsPerPage;
    return filteredAdvertisements.value.slice(start, start + itemsPerPage);
});

const totalPages = computed(() => Math.ceil(filteredAdvertisements.value.length / itemsPerPage));
</script>

<template>
    <div className="AdminPlayerAdvertisements">
        <h1><i class="bi bi-list-nested"></i> Player Advertisements</h1>
        <p></p>
        <div class="d-flex align-items-center mb-3">
            <!-- Szukanie ogłoszenia -->
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
                <label class="form-label"><strong>Status</strong></label>
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
            <table class="table table-striped table-bordered table-hover">
                <thead class="table-dark">
                    <tr>
                        <th>Creation Date</th>
                        <th>Status</th>
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
                        <td colspan="8" class="text-center">No player advertisement available</td>
                    </tr>
                    <tr v-for="advertisement in currentPlayerAdvertisementItems" :key="advertisement.id">
                        <td class="ad-row">{{ TimeService.formatDateToEUR(advertisement.creationDate) }}</td>
                        <td class="ad-row">
                            <i v-if="new Date(advertisement.endDate).getTime() >= Date.now()" class="bi bi-check-circle-fill text-success"></i> 
                            <i v-else class="bi bi-x-circle-fill text-danger"></i> 
                            {{ new Date(advertisement.endDate).getTime() >= Date.now() ? 'Active' : 'Inactive' }}
                        </td>
                        <td class="ad-row">{{ advertisement.player.firstName }} {{ advertisement.player.lastName }}</td>
                        <td class="ad-row">{{ advertisement.playerPosition.positionName }}</td>
                        <td class="ad-row">{{ advertisement.league }}</td>
                        <td class="ad-row">{{ advertisement.region }}</td>
                        <td class="ad-row">{{ advertisement.salaryRange.min }} - {{ advertisement.salaryRange.max }}</td>
                        <td class="ad-row">
                            <button class="btn btn-dark me-2" @click="moveToPlayerAdvertisementPage(advertisement.id)">
                                <i class="bi bi-info-square"></i>
                            </button>
                            <button class="btn btn-secondary me-2" data-bs-toggle="modal" data-bs-target="#finishAdvertisementModal" @click="handleShowFinishModal(advertisement.id)">
                                <i class="bi bi-calendar-x"></i>
                            </button>
                            <button class="btn btn-danger me-2" data-bs-toggle="modal" data-bs-target="#deleteAdvertisementModal" @click="handleShowDeleteModal(advertisement.id)">
                                <i class="bi bi-trash"></i>
                            </button>
                            <template v-if="advertisement.playerId !== userId">
                                <span class="me-2">|</span>
                                <button class="btn btn-info" @click="handleOpenChat(advertisement.playerId)">
                                    <i class="bi bi-chat-fill"></i>
                                </button>
                            </template>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- Modal zakończenia ogłoszenia piłkarskiego -->
        <div class="modal" id="finishAdvertisementModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Confirm action</h5>
                        <button type="button" class="btn-close" @click="closeModal('finishAdvertisementModal')"></button>
                    </div>
                    <div class="modal-body">
                        Are you sure you want to finish this advertisement?
                    </div>
                    <div class="modal-footer">
                        <button class="btn btn-secondary" @click="closeModal('finishAdvertisementModal')">Cancel</button>
                        <button class="btn btn-dark" @click="finishPlayerAdvertisement">Finish</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal usuwania ogłoszenia piłkarskiego -->
        <div class="modal" id="deleteAdvertisementModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Confirm action</h5>
                        <button type="button" class="btn-close" @click="closeModal('deleteAdvertisementModal')"></button>
                    </div>
                    <div class="modal-body">
                        Are you sure you want to delete this advertisement?
                    </div>
                    <div class="modal-footer">
                        <button class="btn btn-secondary" @click="closeModal('deleteAdvertisementModal')">Cancel</button>
                        <button class="btn btn-danger" @click="deleteAdvertisement">Delete</button>
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