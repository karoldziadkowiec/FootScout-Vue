<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useToast } from 'vue-toast-notification';
import { closeModal  } from '../../services/modal/ModalFunction';
import { TimeService } from '../../services/time/TimeService';
import { AccountService } from '../../services/api/AccountService';
import ProblemService from "../../services/api/ProblemService";
import ChatService from "../../services/api/ChatService";
import type { Problem } from "../../models/interfaces/Problem";
import type { ChatCreateDTO } from "../../models/dtos/ChatCreateDTO";
import '../../styles/admin/AdminSupport.css';

// AdminSupports.vue - Komponent zarządzający zgłoszonymi problemami użytkowników

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Zmienne reaktywne przechowujące dane o użytkowniku i problemach
const userId = ref<string | null>(null);
const unsolvedProblems = ref<Problem[]>([]);
const unsolvedProblemCount = ref<number>(0);
const solvedProblems = ref<Problem[]>([]);
const solvedProblemCount = ref<number>(0);
const selectedProblem = ref<Problem | null>(null);
const problemToCheckSolved = ref<Problem | null>(null);
const activeTab = ref("unsolved");    // Aktualnie aktywna zakładka (niesolved/solved)

// Pobieranie danych użytkownika i problemów po załadowaniu komponentu
onMounted(async () => {
  try {
    const id = await AccountService.getId();
    if (id) {
      userId.value = id;
    }
  }
  catch (error) {
    console.error("Failed to fetch user's data:", error);
    toast.error("Failed to load user's data.");
  }

  try {
    // Pobranie listy nierozwiązanych i rozwiązanych problemów oraz ich liczby
    unsolvedProblems.value = await ProblemService.getUnsolvedProblems();
    unsolvedProblemCount.value = await ProblemService.getUnsolvedProblemCount();
    solvedProblems.value = await ProblemService.getSolvedProblems();
    solvedProblemCount.value = await ProblemService.getSolvedProblemCount();
  }
  catch (error) {
    console.error("Failed to fetch reported problems:", error);
    toast.error("Failed to load reported problems.");
  }
});

// Obsługa kliknięcia problemu, aby wyświetlić jego szczegóły
const handleShowProblemDetails = (problem: Problem) => {
  selectedProblem.value = problem;
};

// Obsługa kliknięcia przycisku "Solved", otwierającego modal potwierdzenia rozwiązania problemu
const handleShowCheckProblemSolvedModal = (problem: Problem) => {
  problemToCheckSolved.value = problem;
};

// Zmiana statusu problemu na rozwiązany
const checkProblemSolved = async () => {
  if (!problemToCheckSolved.value || !userId.value)
  return;

  try {
    await ProblemService.checkProblemSolved(problemToCheckSolved.value.id, {
      ...problemToCheckSolved.value,
    });
    toast.success("Reported problem has been set to solved.");

    // Odświeżenie listy problemów po zmianie statusu
    unsolvedProblems.value = await ProblemService.getUnsolvedProblems();
    unsolvedProblemCount.value = await ProblemService.getUnsolvedProblemCount();
    solvedProblems.value = await ProblemService.getSolvedProblems();
    solvedProblemCount.value = await ProblemService.getSolvedProblemCount();

    closeModal('checkProblemSolvedModal');
  }
  catch (error) {
    console.error("Failed to set problem to solved:", error);
    toast.error("Failed to set problem to solved.");
  }
};

// Otwieranie czatu z użytkownikiem, który zgłosił problem
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
    
    router.push(`/chat/${chatId}`);
  }
  catch (error) {
    console.error("Failed to open chat:", error);
    toast.error("Failed to open chat.");
  }
};

// Pobiera nazwę statusu problemu
const getStatusName = (isSolved: boolean): string => {
    if (isSolved === true) {
        return 'Solved';
    }
    else {
        return 'Unsolved';
    }
};

// Eksportowanie danych o problemach do pliku CSV
const exportDataToCSV = async () => {
    await ProblemService.exportProblemsToCsv();
};

</script>
<!-- Struktura strony admina: lista zgłoszonych problemów (rozwiązane i nierozwiązane) oraz szczegóły zgłoszenia -->
<template>
    <div class="AdminSupport">
      <h1><i class="bi bi-cone-striped"></i> Reported Problems</h1>
      
      <button class="btn btn-success mb-3" @click="exportDataToCSV">
        <i class="bi bi-download"></i> Export to CSV
      </button>
  
      <!-- Zakładki (Unsolved / Solved) -->
      <ul class="nav nav-tabs mb-3">
        <li class="nav-item">
          <button class="nav-link" 
            :class="{ active: activeTab === 'unsolved' }" 
            @click="activeTab = 'unsolved'">
            Unsolved Problems
          </button>
        </li>
        <li class="nav-item">
          <button class="nav-link" 
            :class="{ active: activeTab === 'solved' }" 
            @click="activeTab = 'solved'">
            Solved Problems
          </button>
        </li>
      </ul>
  
      <!-- Unsolved Problems -->
      <div v-if="activeTab === 'unsolved'">
        <h3><i class="bi bi-exclamation-diamond"></i> Unsolved problems</h3>
        <h4>Count: <strong>{{ unsolvedProblemCount }}</strong></h4>
  
        <div class="table-responsive">
          <table class="table table-striped table-bordered table-hover table-light">
            <thead class="table-primary">
              <tr>
                <th>Received Date</th>
                <th>Status</th>
                <th>Requester</th>
                <th>Title</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="unsolvedProblems.length === 0">
                <td colspan="5" class="text-center">No unsolved reported problem available</td>
              </tr>
              <tr v-for="(problem, index) in unsolvedProblems" :key="index">
                <td>{{ TimeService.formatDateToEURWithHour(problem.creationDate) }}</td>
                <td>
                  <i v-if="problem.isSolved" class="bi bi-check-circle-fill text-success"></i>
                  <i v-else class="bi bi-x-circle-fill text-danger"></i>
                  {{ getStatusName(problem.isSolved) }}
                </td>
                <td>{{ problem.requester.firstName }} {{ problem.requester.lastName }}</td>
                <td>{{ problem.title }}</td>
                <td>
                  <button class="btn btn-dark me-2" data-bs-toggle="modal" data-bs-target="#problemDetailsModal" @click="handleShowProblemDetails(problem)">
                    <i class="bi bi-info-square"></i> Info
                  </button>
                  <button v-if="!problem.isSolved" class="btn btn-success me-2" data-bs-toggle="modal" data-bs-target="#checkProblemSolvedModal" @click="handleShowCheckProblemSolvedModal(problem)">
                    <i class="bi bi-check-lg"></i> Solved
                  </button>
                  <button v-if="problem.requesterId !== userId" class="btn btn-info" @click="handleOpenChat(problem.requesterId)">
                    <i class="bi bi-chat-fill"></i>
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
  
      <!-- Solved Problems -->
      <div v-if="activeTab === 'solved'">
        <h3><i class="bi bi-check2-circle"></i> Solved problems</h3>
        <h4>Count: <strong>{{ solvedProblemCount }}</strong></h4>
  
        <div class="table-responsive">
          <table class="table table-striped table-bordered table-hover table-light">
            <thead class="table-success">
              <tr>
                <th>Received Date</th>
                <th>Status</th>
                <th>Requester</th>
                <th>Title</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="solvedProblems.length === 0">
                <td colspan="5" class="text-center">No solved reported problem available</td>
              </tr>
              <tr v-for="(problem, index) in solvedProblems" :key="index">
                <td>{{ TimeService.formatDateToEURWithHour(problem.creationDate) }}</td>
                <td>
                  <i v-if="problem.isSolved" class="bi bi-check-circle-fill text-success"></i>
                  <i v-else class="bi bi-x-circle-fill text-danger"></i>
                  {{ getStatusName(problem.isSolved) }}
                </td>
                <td>{{ problem.requester.firstName }} {{ problem.requester.lastName }}</td>
                <td>{{ problem.title }}</td>
                <td>
                  <button class="btn btn-dark me-2" data-bs-toggle="modal" data-bs-target="#problemDetailsModal" @click="handleShowProblemDetails(problem)">
                    <i class="bi bi-info-square"></i> Info
                  </button>
                  <button v-if="problem.requesterId !== userId" class="btn btn-info" @click="handleOpenChat(problem.requesterId)">
                    <i class="bi bi-chat-fill"></i>
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
  
      <!-- Modal szczegółów problemu -->
    <div class="modal" id="problemDetailsModal" tabindex="-1">
      <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Problem Details</h5>
            <button type="button" class="btn-close" @click="closeModal('problemDetailsModal')"></button>
          </div>
          <div class="modal-body" v-if="selectedProblem">
            <p><strong>Received Date:</strong> {{ TimeService.formatDateToEURWithHour(selectedProblem.creationDate) }}</p>
            <p>
              <strong>Problem status: </strong>
              <i v-if="selectedProblem.isSolved" class="bi bi-check-circle-fill text-success"></i>
              <i v-else class="bi bi-x-circle-fill text-danger"></i>
              {{ getStatusName(selectedProblem.isSolved) }}
            </p>
            <p><strong>Title:</strong> {{ selectedProblem.title }}</p>
            <p><strong>Description:</strong> {{ selectedProblem.description }}</p>
            <p><strong>Name:</strong> {{ selectedProblem.requester.firstName }} {{ selectedProblem.requester.lastName }}</p>
            <p><strong>Email:</strong> {{ selectedProblem.requester.email }}</p>
            <p><strong>Phone number:</strong> {{ selectedProblem.requester.phoneNumber }}</p>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="closeModal('problemDetailsModal')">Close</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal do zaznaczania problemu jako rozwiązany -->
    <div class="modal" id="checkProblemSolvedModal" tabindex="-1">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Confirm Action</h5>
            <button type="button" class="btn-close" @click="closeModal('checkProblemSolvedModal')"></button>
          </div>
          <div class="modal-body">
            <p>Are you sure you want to mark this problem as solved?</p>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="closeModal('checkProblemSolvedModal')">Cancel</button>
            <button class="btn btn-success" @click="checkProblemSolved">Accept</button>
          </div>
        </div>
      </div>
    </div>
    </div>  
</template>