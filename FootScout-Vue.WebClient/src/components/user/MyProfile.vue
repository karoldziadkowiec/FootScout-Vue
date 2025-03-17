<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import { closeModal  } from '../../services/modal/ModalFunction';
import { AccountService } from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import { TimeService } from '../../services/time/TimeService';
import type { UserDTO } from '../../models/dtos/UserDTO';
import type { UserUpdateDTO } from '../../models/dtos/UserUpdateDTO';
import type { UserResetPasswordDTO } from '../../models/dtos/UserResetPasswordDTO';
import '../../styles/user/MyProfile.css';

const router = useRouter();
const toast = useToast();

const user = ref<UserDTO | null>(null);
const isAdminRole = ref<boolean | null>(null);
const loading = ref<boolean>(true);

const updateFormData = ref<UserUpdateDTO>({
  firstName: '',
  lastName: '',
  phoneNumber: '',
  location: '',
});

const resetPasswordFormData = ref<UserResetPasswordDTO>({
  passwordHash: '',
  confirmPasswordHash: '',
});

onMounted(async () => {
  try {
    const userId = await AccountService.getId();
    isAdminRole.value = await AccountService.isRoleAdmin();

    if (userId) {
      user.value = await UserService.getUser(userId);
      updateFormData.value = { ...user.value };
    }
  }
  catch (error) {
    console.error('Failed to fetch user data:', error);
    toast.error('Failed to load user data.');
  }
  finally {
    loading.value = false;
  }
});

const handleEditProfile = async () => {
  if (!user.value)
    return;

  const validationError = validateEditForm();
  if (validationError) {
      toast.error(validationError);
      return;
  }

  try {
    await UserService.updateUser(user.value.id, updateFormData.value);
    toast.success('Profile updated successfully!');
    user.value = await UserService.getUser(user.value.id);
    closeModal('editProfileModal');
  } 
  catch (error) {
    console.error('Failed to update user data:', error);
    toast.error('Failed to update user data.');
  }
};

const validateEditForm = () => {
    const { firstName, lastName, phoneNumber, location } = updateFormData.value;

    // Sprawdź czy są puste formsy
    if (!firstName || !lastName || !phoneNumber || !location)
        return 'All fields are required.';

    // walidacja typu numeru telefonu
    if (isNaN(Number(phoneNumber)))
        return 'Phone number must be a number.';

    // walidacja długosci numeru telefonu
    if (phoneNumber.length !== 9)
        return 'Phone number must contain exactly 9 digits.';

    return null;
};

const handleResetPassword = async () => {
  if (!user.value)
    return;

  const validationError = validateResetPasswordForm();
  if (validationError) {
      toast.error(validationError);
      return;
  }

  try {
    await UserService.resetUserPassword(user.value.id, resetPasswordFormData.value);
    toast.success('Password updated successfully! Try to log in with new password.');
    router.push('/');
    closeModal('resetPasswordModal');
  }
  catch (error) {
    console.error('Failed to update user\'s password:', error);
    toast.error('Failed to update user\'s password.');
  }
};

const validateResetPasswordForm = () => {
    const { passwordHash, confirmPasswordHash } = resetPasswordFormData.value;

    // sprawdź czy istnieją niewypełnione formsy
    if (!passwordHash || !confirmPasswordHash)
        return 'All fields are required.';

    // sprawdź długość haseł
    if (passwordHash.length < 7 || confirmPasswordHash.length < 7)
        return 'Password must contain minimum 7 digits.';

    // porównanie haseł
    if (passwordHash !== confirmPasswordHash)
        return 'New passwords do not match.';

    // walidacja hasła
    const passwordError = passwordValidator(passwordHash);
    if (passwordError)
        return passwordError;

    return null;
};

const passwordValidator = (password: string): string | null => {
    const passwordRegex = /^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{7,}$/;
    if (!passwordRegex.test(password))
        return 'Password must be at least 7 characters long, contain at least one uppercase letter, one number, and one special character.';

    return null;
};

const handleDeleteProfile = async () => {
  if (!user.value)
    return;

  try {
    await UserService.deleteUser(user.value.id);
    toast.success('Your account has been deleted successfully.');
    router.push('/');
    closeModal('deleteProfileModal');
  }
  catch (error) {
    console.error('Failed to delete user:', error);
    toast.error('Failed to delete user.');
  }
};
</script>

<template>
  <div class="MyProfile">
    <h1><i class="bi bi-person-fill"></i> My Profile</h1>

    <div class="buttons-container mb-3 row">
      <div class="col">
        <button class="btn btn-success dynamic-button" data-bs-toggle="modal" data-bs-target="#editProfileModal">
          <i class="bi bi-pencil-square"></i> Edit profile
        </button>
      </div>
      <div class="col">
        <button class="btn btn-secondary dynamic-button" data-bs-toggle="modal" data-bs-target="#resetPasswordModal">
          <i class="bi bi-key"></i> Reset password
        </button>
      </div>
      <div class="col" v-if="isAdminRole === false">
        <button class="btn btn-danger dynamic-button" data-bs-toggle="modal" data-bs-target="#deleteProfileModal">
          <i class="bi bi-trash"></i> Delete profile
        </button>
      </div>
    </div>

    <div class="data-container" v-if="user">
      <p class="white-label"><strong>Email:</strong> <span class="green-label">{{ user.email }}</span></p>
      <p class="white-label"><strong>First Name:</strong> <span class="green-label">{{ user.firstName }}</span></p>
      <p class="white-label"><strong>Last Name:</strong> <span class="green-label">{{ user.lastName }}</span></p>
      <p class="white-label"><strong>Phone Number:</strong> <span class="green-label">{{ user.phoneNumber }}</span></p>
      <p class="white-label"><strong>Location:</strong> <span class="green-label">{{ user.location }}</span></p>
      <p class="white-label"><strong>Creation Date:</strong> <span class="green-label">{{ TimeService.formatDateToEURWithHour(user.creationDate) }}</span></p>
    </div>

    <!-- Modal Edycji -->
    <div class="modal" id="editProfileModal" tabindex="-1">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Edit Profile</h5>
            <button type="button" class="btn-close" @click="closeModal('editProfileModal')"></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="handleEditProfile">
              <div class="mb-3">
                <label class="form-label">First Name*</label>
                <input v-model="updateFormData.firstName" placeholder="First Name" required maxlength="20" class="form-control">
              </div>
              <div class="mb-3">
                <label class="form-label">Last Name*</label>
                <input v-model="updateFormData.lastName" placeholder="Last Name" required maxlength="30"  class="form-control">
              </div>
              <div class="mb-3">
                <label class="form-label">Phone Number*</label>
                <input v-model="updateFormData.phoneNumber" placeholder="Phone Number" required maxlength="9"  class="form-control">
              </div>
              <div class="mb-3">
                <label class="form-label">Location*</label>
                <input v-model="updateFormData.location" placeholder="Location" required maxlength="40"  class="form-control">
              </div>
              <div class="modal-footer">
                <button type="button" class="btn btn-secondary" @click="closeModal('editProfileModal')">Cancel</button>
                <button type="submit" class="btn btn-success">Save Changes</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Resetowania Hasła -->
    <div class="modal" id="resetPasswordModal" tabindex="-1">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Reset Password</h5>
            <button type="button" class="btn-close" @click="closeModal('resetPasswordModal')"></button>
          </div>
          <div class="modal-body">
            <p>Enter new password*:</p>
            <input v-model="resetPasswordFormData.passwordHash" type="password"  placeholder="New password" required minlength="7" maxlength="30"  class="form-control">
            <p class="mt-2">Confirm new password*:</p>
            <input v-model="resetPasswordFormData.confirmPasswordHash" type="password"  placeholder="Confirm new password" required minlength="7" maxlength="30"  class="form-control">
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeModal('resetPasswordModal')">Cancel</button>
            <button class="btn btn-success" @click="handleResetPassword">Save</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Usuwania Konta -->
    <div class="modal" id="deleteProfileModal" tabindex="-1">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Confirm action</h5>
            <button type="button" class="btn-close" @click="closeModal('deleteProfileModal')"></button>
          </div>
          <div class="modal-body">
            <p>Are you sure you want to delete your profile?</p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeModal('deleteProfileModal')">Cancel</button>
            <button class="btn btn-danger" @click="handleDeleteProfile">Delete Profile</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>