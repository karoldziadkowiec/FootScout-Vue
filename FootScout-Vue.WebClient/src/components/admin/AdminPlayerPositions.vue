<script setup lang="ts">
  import { ref, onMounted } from "vue";
  import { useToast } from 'vue-toast-notification';
  import { closeModal  } from '../../services/modal/ModalFunction';
  import PlayerPositionService from "../../services/api/PlayerPositionService";
  import type { PlayerPosition } from '../../models/interfaces/PlayerPosition';
  import '../../styles/admin/AdminPlayerPositions.css';

 // AdminPlayerPositions.vue - Komponent zarządzający listą pozycji graczy i tworzeniem nowych pozycji 

  const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)
  
  // Tworzenie reaktywnych zmiennych
  const positions = ref<PlayerPosition[]>([]);  // Reaktywna zmienna przechowująca listę pozycji graczy
  const positionCount = ref<number>(0);         // Reaktywna zmienna przechowująca liczbę pozycji graczy
  const createPositionForm = ref({              // Reaktywna zmienna przechowująca dane formularza dla nowej pozycji
    id: 0,
    positionName: "",
  });
  
  // Ładowanie danych po załadowaniu komponentu (onMounted)
  onMounted(async () => {
    await fetchPositionsData();   // Pobranie danych pozycji graczy z serwera
  });
  
  // Funkcja do pobierania danych pozycji z API
  const fetchPositionsData = async () => {
    try {
      // Pobranie pozycji graczy oraz liczby pozycji z serwisu
      positions.value = await PlayerPositionService.getPlayerPositions();
      positionCount.value = await PlayerPositionService.getPlayerPositionCount();
    }
    catch (error) {
      console.error("Failed to fetch positions:", error);
      toast.error("Failed to load positions.");
    }
  };
  
  // Funkcja do tworzenia nowej pozycji
  const createPosition = async () => {
    if (!createPositionForm.value.positionName) {
      toast.error("Position Name field is required!");
      return;
    }
  
    try {
      // Sprawdzenie, czy pozycja już istnieje
      const isExists = await PlayerPositionService.checkPlayerPositionExists(createPositionForm.value.positionName);
      if (isExists) {
        toast.error("Position Name already exists.");
        return;
      }
  
      // Tworzenie nowej pozycji
      await PlayerPositionService.createPlayerPosition(createPositionForm.value);
      toast.success("Position created successfully!");
      await fetchPositionsData();     // Ponowne załadowanie listy pozycji
      closeModal('createPositionModal')
    }
    catch (error) {
      console.error("Failed to create new position:", error);
      toast.error("Failed to create new position.");
    }
  };

</script>
<!-- Struktura strony admina: wyświetlanie listy pozycji i formularz do tworzenia nowych pozycji -->
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