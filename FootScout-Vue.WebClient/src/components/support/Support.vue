<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useToast } from 'vue-toast-notification';
import { AccountService } from '../../services/api/AccountService';
import ProblemService from '../../services/api/ProblemService';
import type { ProblemCreateDTO } from '../../models/dtos/ProblemCreateDTO';
import '../../styles/support/Support.css';

const toast = useToast();

const userId = ref<string | null>(null);
const problemCreateDTO = ref<ProblemCreateDTO>({
    title: '',
    description: '',
    requesterId: ''
});
const isSubmitted = ref(false);

onMounted(async () => {
  try {
    userId.value = await AccountService.getId();
  }
  catch (error) {
    console.error('Failed to fetch userId:', error);
    toast.error('Failed to load userId.');
  }
});

const validateForm = (formData: ProblemCreateDTO) => {
  if (!formData.title || !formData.description) {
    return 'All fields are required.';
  }
  return null;
};

const handleReportProblem = async () => {
  if (!userId.value)
    return;

  const validationError = validateForm(problemCreateDTO.value);
  if (validationError) {
    toast.error(validationError);
    return;
  }

  try {
    await ProblemService.createProblem({
      ...problemCreateDTO.value,
      requesterId: userId.value,
    });

    isSubmitted.value = true;
  }
  catch (error) {
    console.error('Failed to report problem:', error);
    toast.error('Failed to report problem.');
  }
};
</script>

<template>
  <div class="Support">
    <h1><i class="bi bi-wrench-adjustable"></i> Support</h1>
    <p></p>
    <h3>Report a problem/request</h3>

    <div v-if="!isSubmitted" class="row justify-content-md-center forms-container">
      <div class="col-md-6">
        <form @submit.prevent="handleReportProblem">
          <!-- Title -->
          <div class="mb-3">
            <label for="title" class="white-label">Title</label>
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

          <!-- Description -->
          <div class="mb-3">
            <label for="description" class="white-label">Description</label>
            <textarea
              id="description"
              v-model="problemCreateDTO.description"
              class="form-control"
              placeholder="Enter description"
              maxlength="500"
              required
            ></textarea>
          </div>

          <!-- Submit Button -->
          <button type="submit" class="btn btn-primary">
            <i class="bi bi-flag-fill"></i> Submit
          </button>
        </form>
      </div>
    </div>

    <!-- Success Message -->
    <div v-else class="text-center mt-4 success-container">
      <h5 class="text-success">Problem/request has been reported successfully!</h5>
      <i class="bi bi-check-circle-fill text-success fs-3"></i>
      <p></p>
      <h6><strong>We will try to solve the problem ASAP and contact you if necessary.</strong></h6>
    </div>
  </div>
</template>