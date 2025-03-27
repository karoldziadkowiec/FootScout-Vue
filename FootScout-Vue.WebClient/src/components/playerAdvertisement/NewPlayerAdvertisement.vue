<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import { AccountService } from '../../services/api/AccountService';
import PlayerAdvertisementService from '../../services/api/PlayerAdvertisementService';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import PlayerFootService from '../../services/api/PlayerFootService';
import type { PlayerAdvertisementCreateDTO } from '../../models/dtos/PlayerAdvertisementCreateDTO';
import type { PlayerPosition } from '../../models/interfaces/PlayerPosition';
import type { PlayerFoot } from '../../models/interfaces/PlayerFoot';
import '../../styles/playerAdvertisement/NewPlayerAdvertisement.css';

// NewPlayerAdvertisment.vue - Komponent do tworzenia nowego ogłoszenia o piłkarzu

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Przechowuje ID użytkownika
const userId = ref<string | null>(null);
// Lista dostępnych pozycji zawodników
const positions = ref<PlayerPosition[]>([]);
// Lista możliwych nóg dominujących
const feet = ref<PlayerFoot[]>([]);

// Obiekt przechowujący dane nowego ogłoszenia
const playerAdvertisementDTO = ref<PlayerAdvertisementCreateDTO>({
  playerPositionId: 0,
  league: '',
  region: '',
  age: 0,
  height: 0,
  playerFootId: 0,
  salaryRangeDTO: { min: 0, max: 0 },
  playerId: '',
});

// Pobranie danych użytkownika oraz list dostępnych pozycji i nóg
onMounted(async () => {
  try {
    userId.value = await AccountService.getId();      // Pobranie ID zalogowanego użytkownika
  }
  catch (error) {
    console.error('Failed to fetch userId:', error);
    toast.error('Failed to load userId.');
  }

  try {
    positions.value = await PlayerPositionService.getPlayerPositions();   // Pobranie listy pozycji
  }
  catch (error) {
    console.error('Failed to fetch positions:', error);
    toast.error('Failed to load positions.');
  }

  try {
    feet.value = await PlayerFootService.getPlayerFeet();     // Pobranie listy nóg dominujących
  } 
  catch (error) {
    console.error('Failed to fetch foot names:', error);
    toast.error('Failed to load foot names.');
  }
});

// Funkcja walidująca formularz przed wysłaniem
const validateForm = (formData: PlayerAdvertisementCreateDTO) => {
    const { playerPositionId, league, region, age, height, playerFootId, salaryRangeDTO } = formData;
    const { min, max } = salaryRangeDTO;

    if (!playerPositionId || !league || !region || !age || !height || !playerFootId || !min || !max)
        return 'All fields are required.';

    // Sprawdzenie, czy podane wartości liczbowe są rzeczywiście liczbami
    if (isNaN(Number(age)) || isNaN(Number(height)) || isNaN(Number(min)) || isNaN(Number(max)))
        return 'Age, height, min and max salary must be numbers.';

    // Sprawdzenie, czy wartości liczbowe są nieujemne
    if (Number(age) < 0 || Number(height) < 0 || Number(min) < 0 || Number(max) < 0)
        return 'Age, height, min and max salary must be greater than or equal to 0.';

    // Sprawdzenie, czy maksymalna pensja jest większa niż minimalna
    if (max < min) {
        return 'Max Salary must be greater than Min Salary.';
    }

    return null;
};

// Obsługa tworzenia nowego ogłoszenia
const handleCreate = async () => {
  if (!userId.value)    // Sprawdzenie, czy ID użytkownika jest dostępne
    return;

  const validationError = validateForm(playerAdvertisementDTO.value);
  if (validationError) {
    toast.error(validationError);
    return;
  }

  // Przygotowanie danych do wysłania na serwer
  try {
    const createFormData = { ...playerAdvertisementDTO.value, playerId: userId.value };
    await PlayerAdvertisementService.createPlayerAdvertisement(createFormData);   // Wysłanie żądania do API
    toast.success('Player advertisement created successfully!');
    router.push('/my-player-advertisements');       // Przekierowanie do listy ogłoszeń
  }
  catch (error) {
    console.error('Failed to create player advertisement:', error);
    toast.error('Failed to create player advertisement.');
  }
};

</script>
<!-- Struktura strony: formularz tworzenia ogłoszenia o piłkarzu -->
<template>
<div class="NewPlayerAdvertisement">
    <h1><i class="bi bi-file-earmark-plus"></i> New Player Advertisement</h1>
    <div class="forms-container">
    <div class="row justify-content-md-center">
        <div class="col-md-6">
        <form @submit.prevent="handleCreate">
            <!-- Wybór pozycji zawodnika -->
            <div class="mb-3">
            <label for="position" class="white-label">Position*</label>
            <select v-model="playerAdvertisementDTO.playerPositionId" id="position" class="form-select" required>
                <option v-for="position in positions" :key="position.id" :value="position.id">
                {{ position.positionName }}
                </option>
            </select>
            </div>

            <!-- Pola tekstowe dla ligi i regionu -->
            <div class="row">
            <div class="col">
                <div class="mb-3">
                <label for="league" class="white-label">Preferred League*</label>
                <input type="text" id="league" v-model="playerAdvertisementDTO.league" class="form-control" placeholder="League" maxlength="30" required />
                </div>
            </div>
            <div class="col">
                <div class="mb-3">
                <label for="region" class="white-label">Region*</label>
                <input type="text" id="region" v-model="playerAdvertisementDTO.region" class="form-control" placeholder="Region" maxlength="30" required />
                </div>
            </div>
            </div>

            <!-- Pola numeryczne dla wieku i wzrostu -->
            <div class="row">
            <div class="col">
                <div class="mb-3">
                <label for="age" class="white-label">Age*</label>
                <input type="number" id="age" v-model="playerAdvertisementDTO.age" class="form-control" required />
                </div>
            </div>
            <div class="col">
                <div class="mb-3">
                <label for="height" class="white-label">Height (cm)*</label>
                <input type="number" id="height" v-model="playerAdvertisementDTO.height" class="form-control" required />
                </div>
            </div>
            </div>

            <div class="mb-3">
            <label for="foot" class="white-label">Foot*</label>
            <select v-model="playerAdvertisementDTO.playerFootId" id="foot" class="form-select" required>
                <option v-for="foot in feet" :key="foot.id" :value="foot.id">
                {{ foot.footName }}
                </option>
            </select>
            </div>

            <div class="row">
            <div class="col">
                <div class="mb-3">
                <label for="minSalary" class="white-label">Min Salary (zł.)/month*</label>
                <input type="number" id="minSalary" v-model="playerAdvertisementDTO.salaryRangeDTO.min" class="form-control" required />
                </div>
            </div>
            <div class="col">
                <div class="mb-3">
                <label for="maxSalary" class="white-label">Max Salary (zł.)/month*</label>
                <input type="number" id="maxSalary" v-model="playerAdvertisementDTO.salaryRangeDTO.max" class="form-control" required />
                </div>
            </div>
            </div>

            <button type="submit" class="btn btn-success mb-3">
            <i class="bi bi-file-earmark-plus-fill"></i>
                Create an advertisement
            </button>
        </form>
        </div>
    </div>
    </div>
</div>
</template>