<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { useToast } from 'vue-toast-notification';
import { closeModal  } from '../../services/modal/ModalFunction';
import { AccountService } from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import ClubHistoryService from '../../services/api/ClubHistoryService';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import { TimeService } from '../../services/time/TimeService';
import type { ClubHistoryModel } from '../../models/interfaces/ClubHistory';
import type { PlayerPosition } from '../../models/interfaces/PlayerPosition';
import type { UserDTO } from '../../models/dtos/UserDTO';
import type { ClubHistoryCreateDTO } from '../../models/dtos/ClubHistoryCreateDTO';
import '../../styles/user/ClubHistory.css';

const toast = useToast();
const user = ref<UserDTO | null>(null);
const userClubHistories = ref<ClubHistoryModel[]>([]);
const positions = ref<PlayerPosition[]>([]);
const selectedClubHistory = ref<ClubHistoryModel | null>(null);
const deleteHistoryId = ref<number | null>(null);

const createFormData = reactive<ClubHistoryCreateDTO>({
  playerPositionId: 0,
  clubName: '',
  league: '',
  region: '',
  startDate: '',
  endDate: '',
  achievements: {
    numberOfMatches: 0,
    goals: 0,
    assists: 0,
    additionalAchievements: '',
  },
  playerId: ''
});

const editFormData = ref<ClubHistoryModel | null>(null);

onMounted(async () => {
  try {
    const userId = await AccountService.getId();
    if (userId) {
      user.value = await UserService.getUser(userId);
      userClubHistories.value = await UserService.getUserClubHistory(userId);
    }
    positions.value = await PlayerPositionService.getPlayerPositions();
  }
  catch (error) {
    console.error('Failed to fetch data:', error);
    toast.error('Failed to load data.');
  }
});

const validateForm = (formData: ClubHistoryCreateDTO | ClubHistoryModel) => {
    const { playerPositionId, clubName, league, region, startDate, endDate, achievements } = formData;
    const { numberOfMatches, goals, assists } = achievements;

    if (!playerPositionId || !clubName || !league || !region || !startDate || !endDate)
        return 'All fields are required.';

    if (isNaN(Number(numberOfMatches)) || isNaN(Number(goals)) || isNaN(Number(assists)))
        return 'Matches, goals, and assists must be numbers.';

    if (Number(numberOfMatches) < 0 || Number(goals) < 0 || Number(assists) < 0)
        return 'Matches, goals and assists must be greater than or equal to 0.';

    const start = new Date(startDate);
    const end = new Date(endDate);
    if (end <= start) {
        return 'End date must be later than start date.';
    }

    return null;
};

const handleCreateClubHistory = async () => {
  if (!user.value)
    return;

  const validationError = validateForm(createFormData);
  if (validationError) {
    toast.error(validationError);
    return;
  }

  try {
    createFormData.playerId = user.value.id;
    await ClubHistoryService.createClubHistory(createFormData);
    toast.success('Club history created successfully!');
    userClubHistories.value = await UserService.getUserClubHistory(user.value.id);
    closeModal('createClubHistoryModal');
  }
  catch (error) {
    console.error('Failed to create club history:', error);
    toast.error('Failed to create club history.');
  }
};

const handleShowDetails = (clubHistory: ClubHistoryModel) => {
  selectedClubHistory.value = clubHistory;
};

const handleShowEditModal = (clubHistory: ClubHistoryModel) => {
  editFormData.value = { ...clubHistory };
};

const handleEditClubHistory = async () => {
  if (!user.value || !editFormData.value)
    return;

  const validationError = validateForm(editFormData.value);
  if (validationError) {
    toast.error(validationError);
    return;
  }

  try {
    await ClubHistoryService.updateClubHistory(editFormData.value.id, editFormData.value);
    toast.success('Club history updated successfully!');
    userClubHistories.value = await UserService.getUserClubHistory(user.value.id);
    closeModal('editClubHistoryModal');
  }
  catch (error) {
    console.error('Failed to update club history:', error);
    toast.error('Failed to update club history.');
  }
};

const handleShowDeleteModal = (clubHistoryId: number) => {
  deleteHistoryId.value = clubHistoryId;
};

const handleDeleteClubHistory = async () => {
  if (!user.value || !deleteHistoryId.value)
    return;

  try {
    await ClubHistoryService.deleteClubHistory(deleteHistoryId.value);
    toast.success('Your club history has been deleted successfully.');
    deleteHistoryId.value = null;
    userClubHistories.value = await UserService.getUserClubHistory(user.value.id);
    closeModal('deleteClubHistoryModal');
  }
  catch (error) {
    console.error('Failed to delete club history:', error);
    toast.error('Failed to delete club history.');
  }
};
</script>

<template>
  <div class="ClubHistory">
    <h1><i class="bi bi-clock-history"></i> Club History</h1>
    <button class="btn btn-success dynamic-button" data-bs-toggle="modal" data-bs-target="#createClubHistoryModal">
      <i class="bi bi-file-earmark-plus"></i> Create Club History
    </button>

    <div class="table-responsive mt-3">
      <table class="table table-striped table-bordered table-hover">
        <thead class="table-dark">
          <tr>
            <th>Date</th>
            <th>Club</th>
            <th>League</th>
            <th>Region</th>
            <th>Position</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="userClubHistories.length === 0">
            <td colspan="6" class="text-center">No club history available</td>
          </tr>
          <tr v-for="history in userClubHistories" :key="history.id">
            <td>{{ TimeService.formatDateToEUR(history.startDate) }} - {{ TimeService.formatDateToEUR(history.endDate) }}</td>
            <td>{{ history.clubName }}</td>
            <td>{{ history.league }}</td>
            <td>{{ history.region }}</td>
            <td>{{ history.playerPosition.positionName }}</td>
            <td>
              <button class="btn btn-dark mx-1" data-bs-toggle="modal" data-bs-target="#clubHistoryDetailsModal" @click="handleShowDetails(history)">
                <i class="bi bi-info-square"></i>
              </button>
              <button class="btn btn-warning mx-1" data-bs-toggle="modal" data-bs-target="#editClubHistoryModal" @click="handleShowEditModal(history)">
                <i class="bi bi-pencil-square"></i>
              </button>
              <button class="btn btn-danger mx-1" data-bs-toggle="modal" data-bs-target="#deleteClubHistoryModal" @click="handleShowDeleteModal(history.id)">
                <i class="bi bi-trash"></i>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="modal" id="createClubHistoryModal" tabindex="-1">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="createClubHistoryModalLabel">Create Club History</h5>
            <button type="button" class="btn-close" @click="closeModal('createClubHistoryModal')"></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="handleCreateClubHistory">
              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Position*</label>
                <div class="col-sm-9">
                  <select class="form-select" v-model="createFormData.playerPositionId" required>
                    <option v-for="position in positions" :key="position.id" :value="position.id">
                      {{ position.positionName }}
                    </option>
                  </select>
                </div>
              </div>

              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Club Name*</label>
                <div class="col-sm-9">
                  <input type="text" class="form-control" placeholder="Club Name" v-model="createFormData.clubName" maxlength="30" required />
                </div>
              </div>

              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">League*</label>
                <div class="col-sm-9">
                  <input type="text" class="form-control" placeholder="League" v-model="createFormData.league" maxlength="30" required />
                </div>
              </div>

              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Region*</label>
                <div class="col-sm-9">
                  <input type="text" class="form-control" placeholder="Region" v-model="createFormData.region" maxlength="30" required />
                </div>
              </div>

              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Start Date*</label>
                <div class="col-sm-9">
                  <input type="date" class="form-control" v-model="createFormData.startDate" required />
                </div>
              </div>
              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">End Date*</label>
                <div class="col-sm-9">
                  <input type="date" class="form-control" v-model="createFormData.endDate" required />
                </div>
              </div>

              <!-- Achievements - Osiągnięcia klubowe -->
              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Matches*</label>
                <div class="col-sm-9">
                  <input type="number" class="form-control" v-model.number="createFormData.achievements.numberOfMatches" required />
                </div>
              </div>
              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Goals*</label>
                <div class="col-sm-9">
                  <input type="number" class="form-control" v-model.number="createFormData.achievements.goals" required />
                </div>
              </div>
              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Assists*</label>
                <div class="col-sm-9">
                  <input type="number" class="form-control" v-model.number="createFormData.achievements.assists" required />
                </div>
              </div>

              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Achievements</label>
                <div class="col-sm-9">
                  <textarea class="form-control" v-model="createFormData.achievements.additionalAchievements" placeholder="Your achievements" maxlength="200"></textarea>
                </div>
              </div>

              <div class="modal-footer">
                <button type="button" class="btn btn-secondary" @click="closeModal('createClubHistoryModal')">Close</button>
                <button type="submit" class="btn btn-success">Create</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal szczegółów historii klubowych -->
    <div class="modal" id="clubHistoryDetailsModal" tabindex="-1">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Club History Details</h5>
            <button type="button" class="btn-close" @click="closeModal('clubHistoryDetailsModal')"></button>
          </div>
          <div class="modal-body" v-if="selectedClubHistory">
            <div class="text-center">
              <p><strong class="clubHistory-name-label">{{ selectedClubHistory.clubName.toUpperCase() }}</strong></p>
              <p><strong class="clubHistory-position-label">{{ selectedClubHistory.playerPosition.positionName }}</strong></p>
            </div>
            <div class="row">
              <div class="col">
                <h6 class="clubHistory-section">CLUB INFO</h6>
                <p><strong>League:</strong> {{ selectedClubHistory.league }}</p>
                <p><strong>Region:</strong> {{ selectedClubHistory.region }}</p>
                <p><strong>Start Date:</strong> {{ TimeService.formatDateToEUR(selectedClubHistory.startDate) }}</p>
                <p><strong>End Date:</strong> {{ TimeService.formatDateToEUR(selectedClubHistory.endDate) }}</p>
              </div>
              <div class="col">
                <h6 class="clubHistory-section">ACHIEVEMENTS</h6>
                <p><strong>Matches:</strong> {{ selectedClubHistory.achievements.numberOfMatches }}</p>
                <p><strong>Goals:</strong> {{ selectedClubHistory.achievements.goals }}</p>
                <p><strong>Assists:</strong> {{ selectedClubHistory.achievements.assists }}</p>
                <p><strong>Additional Achievements:</strong> {{ selectedClubHistory.achievements.additionalAchievements }}</p>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="closeModal('clubHistoryDetailsModal')">Close</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal edycji historii klubowej -->
    <div v-if="editFormData" class="modal" id="editClubHistoryModal" tabindex="-1">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Edit Club History</h5>
            <button type="button" class="btn-close" @click="closeModal('editClubHistoryModal')"></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="handleEditClubHistory">
              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Position*</label>
                <div class="col-sm-9">
                  <select class="form-select" v-model="editFormData.playerPositionId" required>
                    <option v-for="position in positions" :key="position.id" :value="position.id">
                      {{ position.positionName }}
                    </option>
                  </select>
                </div>
              </div>

              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Club Name*</label>
                <div class="col-sm-9">
                  <input type="text" class="form-control" v-model="editFormData.clubName" placeholder="Club Name" maxlength="30" required />
                </div>
              </div>

              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">League*</label>
                <div class="col-sm-9">
                  <input type="text" class="form-control" v-model="editFormData.league" placeholder="League" maxlength="30" required />
                </div>
              </div>

              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Region*</label>
                <div class="col-sm-9">
                  <input type="text" class="form-control" v-model="editFormData.region" placeholder="Region" maxlength="30" required />
                </div>
              </div>

              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Start Date*</label>
                <div class="col-sm-9">
                  <input type="date" class="form-control" v-model="editFormData.startDate" required />
                </div>
              </div>

              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">End Date*</label>
                <div class="col-sm-9">
                  <input type="date" class="form-control" v-model="editFormData.endDate" required />
                </div>
              </div>

              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Matches*</label>
                <div class="col-sm-9">
                  <input type="number" class="form-control" v-model.number="editFormData.achievements.numberOfMatches" required />
                </div>
              </div>

              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Goals*</label>
                <div class="col-sm-9">
                  <input type="number" class="form-control" v-model.number="editFormData.achievements.goals" required />
                </div>
              </div>

              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Assists*</label>
                <div class="col-sm-9">
                  <input type="number" class="form-control" v-model.number="editFormData.achievements.assists" required />
                </div>
              </div>

              <div class="mb-3 row">
                <label class="col-sm-3 col-form-label">Achievements</label>
                <div class="col-sm-9">
                  <textarea class="form-control" v-model="editFormData.achievements.additionalAchievements" placeholder="Additional achievements" maxlength="200"></textarea>
                </div>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="closeModal('editClubHistoryModal')">Close</button>
            <button class="btn btn-success" @click="handleEditClubHistory">Update</button>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Modal usuwania historii klubowej -->
    <div class="modal" id="deleteClubHistoryModal" tabindex="-1">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Confirm action</h5>
            <button type="button" class="btn-close" @click="closeModal('deleteClubHistoryModal')"></button>
          </div>
          <div class="modal-body">
            <p>Are you sure you want to delete this club history?</p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeModal('deleteClubHistoryModal')">Cancel</button>
            <button class="btn btn-danger" @click="handleDeleteClubHistory">Delete</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>