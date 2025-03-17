<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import axios from 'axios';
import { AccountService } from '../../services/api/AccountService';
import '../../styles/account/Login.css';

const router = useRouter();
const route = useRoute();
const toast = useToast();

const email = ref<string>('');
const password = ref<string>('');

onMounted(async () => {
  // wyczyść AuthToken w cookies na wejściu
  await AccountService.logout();
  if (route.query.toastMessage) {
    toast.info(route.query.toastMessage as string);
  }
});

const handleLogin = async () => {
  try {
    const loginDTO = { email: email.value, password: password.value };
    await AccountService.login(loginDTO);

    if (await AccountService.isRoleAdmin()) {
      router.push('/admin/dashboard');
    } 
    else {
      router.push('/home');
    }
  } 
  catch (error) {
    if (axios.isAxiosError(error) && error.response) {
      switch (error.response.status) {
        case 401:
          toast.error('Invalid email or password.');
          break;
        case 500:
          toast.error('Internal server error. Please try again later.');
          break;
        default:
          toast.error('Login failed. Please check your credentials and try again.');
      }
    } 
    else {
      toast.error('An unexpected error occurred during login. Please try again.');
    }
  }
};

const moveToRegistrationPage = () => {
  router.push('/registration');
};
</script>

<template>
  <div class="Login">
    <div class="logo-container">
      <img src="../../assets/logo.png" alt="logo" class="logo" />
      <h2 class="text-light">FootScout</h2>
    </div>

    <div class="login-container">
      <form @submit.prevent="handleLogin">
        <h2>Sign in</h2>

        <div class="mb-3">
          <label for="email" class="white-label">E-mail</label>
          <input
            id="email"
            v-model="email"
            type="email"
            class="form-control"
            placeholder="Enter e-mail"
            required
            maxlength="50"
          />
        </div>

        <div class="mb-3">
          <label for="password" class="white-label">Password</label>
          <input
            id="password"
            v-model="password"
            type="password"
            class="form-control"
            placeholder="Enter password"
            required
            maxlength="30"
          />
        </div>

        <button type="submit" class="btn btn-success w-100">
          <i class="bi bi-box-arrow-in-right"></i> Log in
        </button>

        <button type="button" class="btn btn-outline-light mt-3 w-100" @click="moveToRegistrationPage">
          <i class="bi bi-person-plus-fill"></i> Register account
        </button>
      </form>
    </div>
  </div>
</template>