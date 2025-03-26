<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useToast } from 'vue-toast-notification';
import { AccountService } from '../../services/api/AccountService';
import ProblemService from '../../services/api/ProblemService';
import type { ProblemCreateDTO } from '../../models/dtos/ProblemCreateDTO';
import '../../styles/support/Support.css';

// Support.vue - Komponent umożliwiający zgłaszanie problemów przez użytkowników

const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Przechowywanie identyfikatora użytkownika
const userId = ref<string | null>(null);

// Obiekt przechowujący dane dotyczące zgłaszanego problemu
const problemCreateDTO = ref<ProblemCreateDTO>({
    title: '',
    description: '',
    requesterId: ''
});

// Flaga wskazująca, czy zgłoszenie zostało wysłane
const isSubmitted = ref(false);

// Pobranie identyfikatora użytkownika po zamontowaniu komponentu
onMounted(async () => {
  try {
    userId.value = await AccountService.getId();
  }
  catch (error) {
    console.error('Failed to fetch userId:', error);
    toast.error('Failed to load userId.');
  }
});

// Walidacja formularza przed wysłaniem zgłoszenia
const validateForm = (formData: ProblemCreateDTO) => {
  if (!formData.title || !formData.description) {
    return 'All fields are required.';
  }
  return null;
};

// Obsługa zgłaszania problemu
const handleReportProblem = async () => {
  if (!userId.value)
    return;

  // Sprawdzenie poprawności formularza
  const validationError = validateForm(problemCreateDTO.value);
  if (validationError) {
    toast.error(validationError);
    return;
  }

  try {
    // Wysłanie danych problemu do API
    await ProblemService.createProblem({
      ...problemCreateDTO.value,
      requesterId: userId.value,
    });

    isSubmitted.value = true;     // Oznaczenie zgłoszenia jako wysłane
  }
  catch (error) {
    console.error('Failed to report problem:', error);
    toast.error('Failed to report problem.');
  }
};

</script>
<!-- Struktura strony wsparcia: formularz zgłoszeniowy i komunikat sukcesu -->
<template>
  <div class="Support">
    <h1><i class="bi bi-wrench-adjustable"></i> Support</h1>
    <p></p>
    <h3>Report a problem/request</h3>

    <div v-if="!isSubmitted" class="row justify-content-md-center forms-container">
      <div class="col-md-6">
        <form @submit.prevent="handleReportProblem">
          <!-- Tytuł -->
          <div class="mb-3">
            <label for="title" class="white-label">Title*</label>
            <input
              type="text"
              id="title"
              v-model="problemCreateDTO.title"
              class="form-control"
              placeholder="Enter title"
              maxlength="30"
              required
            />
          </div>

          <!-- Opis -->
          <div class="mb-3">
            <label for="description" class="white-label">Description*</label>
            <textarea
              id="description"
              v-model="problemCreateDTO.description"
              class="form-control"
              placeholder="Enter description"
              maxlength="500"
              required
            ></textarea>
          </div>

          <!-- Przycisk do zgłoszenia problemu -->
          <button type="submit" class="btn btn-primary">
            <i class="bi bi-flag-fill"></i> Submit
          </button>
        </form>
      </div>
    </div>

    <!-- Kontener wiadomości o poprawnym zgłoszeniu problemu -->
    <div v-else class="text-center mt-4 success-container">
      <h5 class="text-success">Problem/request has been reported successfully!</h5>
      <i class="bi bi-check-circle-fill text-success fs-3"></i>
      <p></p>
      <h6><strong>We will try to solve the problem ASAP and contact you if necessary.</strong></h6>
    </div>
  </div>
</template>