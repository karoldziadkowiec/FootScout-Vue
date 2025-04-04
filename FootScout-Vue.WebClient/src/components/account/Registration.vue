<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import { AccountService } from '../../services/api/AccountService';
import type { RegisterDTO } from '../../models/dtos/RegisterDTO';
import '../../styles/account/Registration.css';

//Registration.vue - Komponent obsługujący rejestrację nowego użytkownika 

const router = useRouter();  // Router do nawigacji między stronami
const toast = useToast();    // Obsługa powiadomień

// Obiekt rejestracyjny przechowujący dane użytkownika
const registerDTO = ref<RegisterDTO>({
  email: '',
  password: '',
  confirmPassword: '',
  firstName: '',
  lastName: '',
  phoneNumber: '',
  location: ''
});

// Wylogowanie użytkownika na wejściu
onMounted(async () => {
    // wyczyść AuthToken w cookies na wejściu
    await AccountService.logout();
});

// Walidacja formularza przed wysłaniem
const handleRegister = async () => {
  const validationError = validateForm();
  if (validationError) {
    toast.error(validationError);
    return;
  }

  // Rejestracja nowego użytkownika w serwisie  
  try {
    await AccountService.registerUser(registerDTO.value);
    toast.success('Your account has been successfully registered!');
    router.push('/');
  } 
  catch (error) {
    toast.error('Registration failed. Please try again.');
  }
};

// Funkcja sprawdzająca poprawność danych w formularzu
const validateForm = () => {
    const { email, password, confirmPassword, firstName, lastName, phoneNumber, location } = registerDTO.value;

    // Sprawdzenie, czy wszystkie pola są wypełnione
    if (!email || !password || !confirmPassword || !firstName || !lastName || !phoneNumber || !location)
        return 'All fields are required.';

    // Walidacja E-mail
    const emailError = emailValidator(email);
    if (emailError)
        return emailError;

    // Walidacja hasła
    const passwordError = passwordValidator(password);
    if (passwordError)
        return passwordError;

    // Sprawdzenie zgodności haseł
    if (password !== confirmPassword)
        return 'Passwords do not match.';

    // Walidacja typu numeru telefonu
    if (isNaN(Number(phoneNumber)))
        return 'Phone number must be a number.';

    // Sprawdzenie poprawności długości numeru telefonu
    if (phoneNumber.length !== 9)
        return 'Phone number must contain exactly 9 digits.';

    return null;
};

// Funkcja sprawdzająca poprawność e-maila
const emailValidator = (email: string): string | null => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email))
        return 'Invalid email format. Must contain "@" and "."';

    return null;
};

// Funkcja sprawdzająca poprawność hasła
const passwordValidator = (password: string): string | null => {
    const passwordRegex = /^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{7,}$/;
    if (!passwordRegex.test(password))
        return 'Password must be at least 7 characters long, contain at least one uppercase letter, one number, and one special character.';

    return null;
};

</script>
<!-- Struktura strony rejestracji użytkownika -->
<template>
  <div class="Registration">
    <div class="logo-container">
      <img src="../../assets/logo.png" alt="logo" class="logo" />
      <h2 class="text-light">FootScout</h2>
    </div>

    <div class="registration-container">
      <form @submit.prevent="handleRegister">
        <h2 class="text-center mb-3">Sign Up</h2>

        <div class="mb-3">
          <label for="email" class="white-label">E-mail*</label>
          <input id="email" v-model="registerDTO.email" type="email" class="form-control" placeholder="E-mail" required maxlength="50" />
        </div>

        <div class="row">
          <div class="col">
            <div class="mb-3">
              <label for="password" class="white-label">Password*</label>
              <input id="password" v-model="registerDTO.password" type="password" class="form-control" placeholder="Password" required minlength="7" maxlength="30" />
            </div>
          </div>
          <div class="col">
            <div class="mb-3">
              <label for="confirmPassword" class="white-label">Confirm Password*</label>
              <input id="confirmPassword" v-model="registerDTO.confirmPassword" type="password" class="form-control" placeholder="Confirm Password" required minlength="7" maxlength="30" />
            </div>
          </div>
        </div>

        <div class="row">
          <div class="col">
            <div class="mb-3">
              <label for="firstName" class="white-label">First Name*</label>
              <input id="firstName" v-model="registerDTO.firstName" type="text" class="form-control" placeholder="First Name" required maxlength="20" />
            </div>
          </div>
          <div class="col">
            <div class="mb-3">
              <label for="lastName" class="white-label">Last Name*</label>
              <input id="lastName" v-model="registerDTO.lastName" type="text" class="form-control" placeholder="Last Name" required maxlength="30" />
            </div>
          </div>
        </div>

        <div class="row">
          <div class="col">
            <div class="mb-3">
              <label for="phoneNumber" class="white-label">Phone Number*</label>
              <input id="phoneNumber" v-model="registerDTO.phoneNumber" type="tel" class="form-control" placeholder="Phone Number" required maxlength="9" />
            </div>
          </div>
          <div class="col">
            <div class="mb-3">
              <label for="location" class="white-label">Location*</label>
              <input id="location" v-model="registerDTO.location" type="text" class="form-control" placeholder="Location" required maxlength="40" />
            </div>
          </div>
        </div>

        <button type="submit" class="btn btn-success w-100">
          <i class="bi bi-person-plus-fill"></i> Register account
        </button>

        <button type="button" class="btn btn-outline-light mt-3 w-100" @click="router.push('/')">
          Back
        </button>
      </form>
    </div>
  </div>
</template>