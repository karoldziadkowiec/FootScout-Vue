<script setup lang="ts">
  import { ref, onMounted } from "vue";
  import { useToast } from 'vue-toast-notification';
  import { closeModal  } from '../../services/modal/ModalFunction';
  import PlayerPositionService from "../../services/api/PlayerPositionService";
  import type { PlayerPosition } from '../../models/interfaces/PlayerPosition';
  import '../../styles/admin/AdminPlayerPositions.css';

  const toast = useToast();
  const positions = ref<PlayerPosition[]>([]);
  const positionCount = ref<number>(0);
  const createPositionForm = ref({
    id: 0,
    positionName: "",
  });
  
  onMounted(async () => {
    await fetchPositionsData();
  });
  
  const fetchPositionsData = async () => {
    try {
      positions.value = await PlayerPositionService.getPlayerPositions();
      positionCount.value = await PlayerPositionService.getPlayerPositionCount();
    }
    catch (error) {
      console.error("Failed to fetch positions:", error);
      toast.error("Failed to load positions.");
    }
  };
  
  const createPosition = async () => {
    if (!createPositionForm.value.positionName) {
      toast.error("Position Name field is required!");
      return;
    }
  
    try {
      const isExists = await PlayerPositionService.checkPlayerPositionExists(createPositionForm.value.positionName);
      if (isExists) {
        toast.error("Position Name already exists.");
        return;
      }
  
      await PlayerPositionService.createPlayerPosition(createPositionForm.value);
      toast.success("Position created successfully!");
      await fetchPositionsData();
      closeModal('createPositionModal')
    }
    catch (error) {
      console.error("Failed to create new position:", error);
      toast.error("Failed to create new position.");
    }
  };
</script>

<template>
    <div class="AdminPlayerPositions">
      <h1><i class="bi bi-person-standing"></i> Player Positions</h1>
      <p></p>
      <h3>Count: <strong>{{ positionCount }}</strong></h3>
      <p></p>
      <button class="btn btn-primary form-button" data-bs-toggle="modal" data-bs-target="#createPositionModal">
        <i class="bi bi-file-earmark-plus"></i>
        Create New Position
      </button>
  
      <div class="table-responsive mt-3">
        <table class="table table-striped table-bordered table-hover table-secondary">
          <thead class="table-dark">
            <tr>
              <th>Position</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="positions.length === 0">
              <td class="text-center" colspan="1">No positions available</td>
            </tr>
            <tr v-for="(position, index) in positions" :key="index">
              <td>{{ position.positionName }}</td>
            </tr>
          </tbody>
        </table>
      </div>
  
      <!-- Modal tworzenia nowej pozycji -->
      <div class="modal" id="createPositionModal" tabindex="-1">
        <div class="modal-dialog">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">Create New Position</h5>
              <button type="button" class="btn-close" @click="closeModal('createPositionModal')"></button>
            </div>
            <div class="modal-body">
              <form @submit.prevent="createPosition">
                <div class="mb-3">
                  <label for="positionName" class="form-label">Position Name*</label>
                  <input
                    id="positionName"
                    type="text"
                    class="form-control"
                    placeholder="Position Name"
                    v-model="createPositionForm.positionName"
                    maxlength="30"
                    required
                  />
                </div>
                <button type="submit" class="btn btn-primary">Create</button>
                <button type="button" class="btn btn-secondary ms-2" @click="closeModal('createPositionModal')">Close</button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
</template>