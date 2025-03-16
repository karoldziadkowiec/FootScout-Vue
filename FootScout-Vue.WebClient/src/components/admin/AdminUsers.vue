<script setup lang="ts">
import { ref, reactive, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useToast } from 'vue-toast-notification';
import { closeModal  } from '../../services/modal/ModalFunction';
import { TimeService } from '../../services/time/TimeService';
import { AccountService } from '../../services/api/AccountService';
import { Role } from '../../models/enums/Role';
import UserService from '../../services/api/UserService';
import ChatService from '../../services/api/ChatService';
import type { ClubHistoryModel } from '../../models/interfaces/ClubHistory';
import type { UserDTO } from '../../models/dtos/UserDTO';
import type { UserUpdateDTO } from '../../models/dtos/UserUpdateDTO';
import type { RegisterDTO } from '../../models/dtos/RegisterDTO';
import type { ChatCreateDTO } from '../../models/dtos/ChatCreateDTO';
import '../../styles/admin/AdminUsers.css';

const toast = useToast();
const router = useRouter();

const userId = ref<string | null>(null);
const roles = ref<string[]>([]);
const users = ref<UserDTO[]>([]);
const onlyUsers = ref<UserDTO[]>([]);
const onlyAdmins = ref<UserDTO[]>([]);
const userRoles = ref<Record<string, string>>({});
const selectedClubHistory = ref<ClubHistoryModel[]>([]);
const editedUserId = ref<string | null>(null);
const deleteUserId = ref<string | null>(null);
const createDTO = reactive<RegisterDTO>({
  email: "",
  password: "",
  confirmPassword: "",
  firstName: "",
  lastName: "",
  phoneNumber: "",
  location: "",
});

const updateFormData = ref<UserUpdateDTO>({
  firstName: "",
  lastName: "",
  phoneNumber: "",
  location: "",
});

// Wyszukiwanie i sortowanie
const searchTerm = ref<string>("");
const selectedRole = ref<string>("All Roles");
const sortCriteria = ref<string>("creationDateDesc");
// Paginacja
const currentPage = ref<number>(1);
const itemsPerPage = 20;
const indexOfLastItem = computed(() => currentPage.value * itemsPerPage);
const indexOfFirstItem = computed(() => indexOfLastItem.value - itemsPerPage);

onMounted(async () => {
  try {
    userId.value = await AccountService.getId();
    roles.value = await AccountService.getRoles();
    await fetchUsers();
  }
  catch (error) {
    console.error("Failed to fetch data:", error);
    toast.error("Failed to load data.");
  }
});

const fetchUsers = async () => {
  try {
    users.value = await UserService.getUsers();
    onlyUsers.value = await UserService.getOnlyUsers();
    onlyAdmins.value = await UserService.getOnlyAdmins();

    const userRoleMap: Record<string, string> = {};
    const userRolesData = await Promise.all(
      users.value.map(async (user) => ({
        userId: user.id,
        role: await UserService.getUserRole(user.id),
      }))
    );

    userRolesData.forEach(({ userId, role }) => {
      userRoleMap[userId] = role;
    });

    userRoles.value = userRoleMap;
  }
  catch (error) {
    console.error("Failed to fetch users:", error);
    toast.error("Failed to load users.");
  }
};

const handlePageChange = (pageNumber: number) => {
  currentPage.value = pageNumber;
};

const register = async () => {
  const validationError = validateForm();
  if (validationError) {
    toast.error(validationError);
    return;
  }

  try {
    await AccountService.registerUser(createDTO);
    toast.success("Account has been successfully created!");
    await fetchUsers();
    closeModal('registerModal')
  }
  catch (error) {
    toast.error("Creation failed. Please try again.");
  }
};

const validateForm = () => {
    const { email, password, confirmPassword, firstName, lastName, phoneNumber, location } = createDTO;

    // Checking empty fields
    if (!email || !password || !confirmPassword || !firstName || !lastName || !phoneNumber || !location)
        return 'All fields are required.';

    // E-mail validation
    const emailError = emailValidator(email);
    if (emailError)
        return emailError;

    // Password validation
    const passwordError = passwordValidator(password);
    if (passwordError)
        return passwordError;

    // Passwords matcher
    if (password !== confirmPassword)
        return 'Passwords do not match.';

    // Checking phone number type
    if (isNaN(Number(phoneNumber)))
        return 'Phone number must be a number.';

    // Checking phone number length
    if (phoneNumber.length !== 9)
        return 'Phone number must contain exactly 9 digits.';

    return null;
};

const emailValidator = (email: string): string | null => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email))
        return 'Invalid email format. Must contain "@" and "."';

    return null;
};

const passwordValidator = (password: string): string | null => {
    const passwordRegex = /^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{7,}$/;
    if (!passwordRegex.test(password))
        return 'Password must be at least 7 characters long, contain at least one uppercase letter, one number, and one special character.';

    return null;
};

const handleShowEditModal = async (userId: string) => {
    editedUserId.value = userId;
    updateFormData.value = await UserService.getUser(userId);
};

const editProfile = async () => {
  if (!editedUserId.value)
    return;

  const validationError = validateEditForm();
  if (validationError) {
    toast.error(validationError);
    return;
  }

  try {
    await UserService.updateUser(editedUserId.value, updateFormData.value);
    toast.success("Profile updated successfully!");
    await fetchUsers();
    closeModal('editProfileModal')
  }
  catch (error) {
    console.error("Failed to update user:", error);
    toast.error("Failed to update user.");
  }
};

const validateEditForm = () => {
    const { firstName, lastName, phoneNumber, location } = updateFormData.value;

    // Checking empty fields
    if (!firstName || !lastName || !phoneNumber || !location)
        return 'All fields are required.';

    // Checking phone number type
    if (isNaN(Number(phoneNumber)))
        return 'Phone number must be a number.';

    // Checking phone number length
    if (phoneNumber.length !== 9)
        return 'Phone number must contain exactly 9 digits.';

    return null;
};

const handleShowDeleteModal = (userId: string) => {
    deleteUserId.value = userId;
};

const deleteProfile = async () => {
  if (!deleteUserId.value)
    return;

  try {
    await UserService.deleteUser(deleteUserId.value);
    toast.success("Account has been deleted successfully.");
    await fetchUsers();
    closeModal('deleteProfileModal')
  }
  catch (error) {
    console.error("Failed to delete user:", error);
    toast.error("Failed to delete user.");
  }
};

const handleShowClubHistoryDetails = async (userId: string) => {
    selectedClubHistory.value = await UserService.getUserClubHistory(userId);
};

const handleOpenChat = async (receiverId: string) => {
  if (!receiverId || !userId.value)
  return;

  try {
    let chatId = await ChatService.getChatIdBetweenUsers(userId.value, receiverId);
    if (chatId === 0) {
      const chatCreateDTO: ChatCreateDTO = { user1Id: userId.value, user2Id: receiverId };
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

const searchUsers = (users: UserDTO[]) => {
    if (!searchTerm) {
        return users;
    }
    const lowerCaseSearchTerm = searchTerm.value.toLowerCase();
    return users.filter(user =>
        (user.firstName + ' ' + user.lastName).toLowerCase().includes(lowerCaseSearchTerm) ||
        user.email.toLowerCase().includes(lowerCaseSearchTerm) ||
        user.phoneNumber.toLowerCase().includes(lowerCaseSearchTerm) ||
        user.location.toLowerCase().includes(lowerCaseSearchTerm)
    );
};

const filterUsersByRole = (users: UserDTO[]): UserDTO[]  => {
    if (selectedRole.value === "All Roles") {
        return users;
    }
    else if (selectedRole.value === "Admin") {
        return onlyAdmins.value;
    }
    else {
        return onlyUsers.value;
    }
};

const sortUsers = (users: UserDTO[]) => {
    switch (sortCriteria.value) {
        case 'creationDateAsc':
            return [...users].sort((a, b) => new Date(a.creationDate).getTime() - new Date(b.creationDate).getTime());
        case 'creationDateDesc':
            return [...users].sort((a, b) => new Date(b.creationDate).getTime() - new Date(a.creationDate).getTime());
        case 'firstNameAsc':
            return [...users].sort((a, b) => a.firstName.localeCompare(b.firstName));
        case 'firstNameDesc':
            return [...users].sort((a, b) => b.firstName.localeCompare(a.firstName));
        case 'lastNameAsc':
            return [...users].sort((a, b) => a.lastName.localeCompare(b.lastName));
        case 'lastNameDesc':
            return [...users].sort((a, b) => b.lastName.localeCompare(a.lastName));
        case 'emailAsc':
            return [...users].sort((a, b) => a.email.localeCompare(b.email));
        case 'emailDesc':
            return [...users].sort((a, b) => b.email.localeCompare(a.email));
        case 'phoneNumberAsc':
            return [...users].sort((a, b) => a.phoneNumber.localeCompare(b.phoneNumber));
        case 'phoneNumberDesc':
            return [...users].sort((a, b) => b.phoneNumber.localeCompare(a.phoneNumber));
        case 'locationAsc':
            return [...users].sort((a, b) => a.location.localeCompare(b.location));
        case 'locationDesc':
            return [...users].sort((a, b) => a.location.localeCompare(b.location));
        default:
            return users;
    }
};

const filteredUsers = computed(() => filterUsersByRole(users.value));
const searchedUsers = computed(() => searchUsers(filteredUsers.value));
const sortedUsers = computed(() => sortUsers(searchedUsers.value));
const currentUserItems = computed(() => sortedUsers.value.slice(indexOfFirstItem.value, indexOfLastItem.value));
const totalPages = computed(() => Math.ceil(searchedUsers.value.length / itemsPerPage));

</script>

<template>
    <div class="AdminUsers">
      <h1><i class="bi bi-people-fill"></i> Users</h1>
      <button class="btn btn-primary form-button" data-bs-toggle="modal" data-bs-target="#registerModal">
        <i class="bi bi-person-plus-fill"></i> Create User
      </button>
      <p></p>
  
      <div class="d-flex align-items-center mb-3">
        <!-- Search -->
        <div>
          <label class="form-label"><strong>Search</strong></label>
          <input 
            type="text" 
            class="form-control" 
            placeholder="Search" 
            v-model="searchTerm"
          />
        </div>
        <!-- Filter Roles -->
        <div class="ms-auto">
          <label class="form-label"><strong>Filter Roles</strong></label>
          <select class="form-select" v-model="selectedRole">
            <option value="All Roles">All Roles</option>
            <option v-for="role in roles" :key="role">{{ role }}</option>
          </select>
        </div>
        <!-- Sort -->
        <div class="ms-auto">
          <label class="form-label"><strong>Sort by</strong></label>
          <select class="form-select" v-model="sortCriteria">
            <option value="creationDateAsc">Creation Date Ascending</option>
            <option value="creationDateDesc">Creation Date Descending</option>
            <option value="firstNameAsc">First Name Ascending</option>
            <option value="firstNameDesc">First Name Descending</option>
            <option value="lastNameAsc">Last Name Ascending</option>
            <option value="lastNameDesc">Last Name Descending</option>
            <option value="emailAsc">E-mail Ascending</option>
            <option value="emailDesc">E-mail Descending</option>
            <option value="phoneNumberAsc">Phone Number Ascending</option>
            <option value="phoneNumberDesc">Phone Number Descending</option>
            <option value="salaryAsc">Location Ascending</option>
            <option value="salaryDesc">Location Descending</option>
          </select>
        </div>
      </div>
  
      <!-- Users Table -->
      <div class="table-responsive">
        <table class="table table-striped table-bordered table-hover table-light">
          <thead class="table-dark">
            <tr>
              <th>Creation Date</th>
              <th>E-mail</th>
              <th>First Name</th>
              <th>Last Name</th>
              <th>Phone number</th>
              <th>Location</th>
              <th>Role</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="currentUserItems.length === 0">
              <td colspan="8" class="text-center">No user available</td>
            </tr>
            <tr v-for="(user, index) in currentUserItems" :key="index">
              <td>{{ TimeService.formatDateToEURWithHour(user.creationDate) }}</td>
              <td>{{ user.email }}</td>
              <td>{{ user.firstName }}</td>
              <td>{{ user.lastName }}</td>
              <td>{{ user.phoneNumber }}</td>
              <td>{{ user.location }}</td>
              <td>{{ userRoles[user.id] || 'Loading...' }}</td>
              <td>
                <button class="btn btn-warning me-2" data-bs-toggle="modal" data-bs-target="#editProfileModal" @click="handleShowEditModal(user.id)">
                  <i class="bi bi-pencil-square"></i>
                </button>
                <button v-if="user.id !== userId && userRoles[user.id] === Role.Admin" 
                        class="btn btn-danger me-2" 
                        data-bs-toggle="modal"
                        data-bs-target="#deleteProfileModal"
                        @click="handleShowDeleteModal(user.id)">
                  <i class="bi bi-trash"></i>
                </button>
                <button v-if="userRoles[user.id] === Role.User" 
                        class="btn btn-danger me-2" 
                        data-bs-toggle="modal"
                        data-bs-target="#deleteProfileModal"
                        @click="handleShowDeleteModal(user.id)">
                  <i class="bi bi-trash"></i>
                </button>
                <button v-if="userRoles[user.id] === Role.User" 
                        class="btn btn-dark me-2"
                        data-bs-toggle="modal"
                        data-bs-target="#profileDetailsModal"
                        @click="handleShowClubHistoryDetails(user.id)">
                  <i class="bi bi-clock-history"></i> Club History
                </button>
                <button v-if="user.id !== userId" 
                        class="btn btn-info" 
                        @click="handleOpenChat(user.id)">
                  <i class="bi bi-chat-fill"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
  
      <!-- Create User Modal -->
      <div class="modal" id="registerModal" tabindex="-1">
        <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
            <h5 class="modal-title">Create User</h5>
            <button type="button" class="btn-close" @click="closeModal('registerModal')"></button>
            </div>
            <div class="modal-body">
            <div class="row justify-content-md-center">
                <div class="col-md-6">
                <form>
                    <div class="mb-3">
                    <label class="form-label"><strong>E-mail*</strong></label>
                    <input type="email" class="form-control" v-model="createDTO.email" placeholder="E-mail" maxlength="50" required />
                    </div>
                    <div class="row">
                    <div class="col">
                        <div class="mb-3">
                        <label class="form-label"><strong>Password*</strong></label>
                        <input type="password" class="form-control" v-model="createDTO.password" placeholder="Password" minlength="7" maxlength="30" required />
                        </div>
                    </div>
                    <div class="col">
                        <div class="mb-3">
                        <label class="form-label"><strong>Confirm Password*</strong></label>
                        <input type="password" class="form-control" v-model="createDTO.confirmPassword" placeholder="Confirm Password" minlength="7" maxlength="30" required />
                        </div>
                    </div>
                    </div>
                    <div class="row">
                    <div class="col">
                        <div class="mb-3">
                        <label class="form-label"><strong>First Name*</strong></label>
                        <input type="text" class="form-control" v-model="createDTO.firstName" placeholder="First Name" maxlength="20" required />
                        </div>
                    </div>
                    <div class="col">
                        <div class="mb-3">
                        <label class="form-label"><strong>Last Name*</strong></label>
                        <input type="text" class="form-control" v-model="createDTO.lastName" placeholder="Last Name" maxlength="30" required />
                        </div>
                    </div>
                    </div>
                    <div class="row">
                    <div class="col">
                        <div class="mb-3">
                        <label class="form-label"><strong>Phone Number*</strong></label>
                        <input type="tel" class="form-control" v-model="createDTO.phoneNumber" placeholder="Phone Number" maxlength="9" required />
                        </div>
                    </div>
                    <div class="col">
                        <div class="mb-3">
                        <label class="form-label"><strong>Location*</strong></label>
                        <input type="text" class="form-control" v-model="createDTO.location" placeholder="Location" maxlength="40" required />
                        </div>
                    </div>
                    </div>
                </form>
                </div>
            </div>
            </div>
            <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeModal('registerModal')">Cancel</button>
            <button type="button" class="btn btn-primary" @click="register">Create</button>
            </div>
        </div>
        </div>
    </div>

    <!-- Edit Profile Modal -->
    <div class="modal" id="editProfileModal" tabindex="-1">
        <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
            <h5 class="modal-title">Edit Profile</h5>
            <button type="button" class="btn-close" @click="closeModal('editProfileModal')"></button>
            </div>
            <div class="modal-body">
            <form>
                <div class="mb-3">
                <label class="form-label">First Name*</label>
                <input type="text" class="form-control" v-model="updateFormData.firstName" maxlength="20" required />
                </div>
                <div class="mb-3">
                <label class="form-label">Last Name*</label>
                <input type="text" class="form-control" v-model="updateFormData.lastName" maxlength="30" required />
                </div>
                <div class="mb-3">
                <label class="form-label">Phone Number*</label>
                <input type="tel" class="form-control" v-model="updateFormData.phoneNumber" maxlength="9" required />
                </div>
                <div class="mb-3">
                <label class="form-label">Location*</label>
                <input type="text" class="form-control" v-model="updateFormData.location" maxlength="40" required />
                </div>
            </form>
            </div>
            <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeModal('editProfileModal')">Cancel</button>
            <button type="button" class="btn btn-primary" @click="editProfile">Save Changes</button>
            </div>
        </div>
        </div>
    </div>

    <!-- Delete User Modal -->
    <div class="modal" id="deleteProfileModal" tabindex="-1">
        <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
            <h5 class="modal-title">Confirm action</h5>
            <button type="button" class="btn-close" @click="closeModal('deleteProfileModal')"></button>
            </div>
            <div class="modal-body">
            Are you sure you want to delete this user?
            </div>
            <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeModal('deleteProfileModal')">Cancel</button>
            <button type="button" class="btn btn-danger" @click="deleteProfile">Delete</button>
            </div>
        </div>
        </div>
    </div>

    <!-- Details of Club History Modal -->
    <div class="modal" id="profileDetailsModal" tabindex="-1">
        <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
            <h5 class="modal-title">Club History Details</h5>
            <button type="button" class="btn-close" @click="closeModal('profileDetailsModal')"></button>
            </div>
            <div class="modal-body">
            <table class="table table-striped table-bordered table-hover">
                <thead class="table-dark">
                <tr>
                    <th>Date</th>
                    <th>Club</th>
                    <th>League (Region)</th>
                    <th>Position</th>
                    <th>Matches</th>
                    <th>Goals</th>
                    <th>Assists</th>
                    <th>Additional info</th>
                </tr>
                </thead>
                <tbody>
                <tr v-if="selectedClubHistory.length === 0">
                    <td colspan="8" class="text-center">No club history available</td>
                </tr>
                <tr v-for="(history, index) in selectedClubHistory" :key="index">
                    <td>{{ TimeService.formatDateToEUR(history.startDate) }} - {{ TimeService.formatDateToEUR(history.endDate) }}</td>
                    <td>{{ history.clubName }}</td>
                    <td>{{ history.league }} ({{ history.region }})</td>
                    <td>{{ history.playerPosition.positionName }}</td>
                    <td>{{ history.achievements.numberOfMatches }}</td>
                    <td>{{ history.achievements.goals }}</td>
                    <td>{{ history.achievements.assists }}</td>
                    <td>{{ history.achievements.additionalAchievements }}</td>
                </tr>
                </tbody>
            </table>
            </div>
            <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeModal('profileDetailsModal')">Close</button>
            </div>
        </div>
        </div>
    </div>

     <!-- Pagination -->
     <nav class="admin-pagination-container">
        <ul class="pagination pagination-blue">
            <li class="page-item" :class="{ disabled: currentPage === 1 }">
            <button class="page-link" @click="handlePageChange(currentPage - 1)">Previous</button>
            </li>
            <li v-for="n in totalPages" :key="n" class="page-item" :class="{ active: n === currentPage }">
            <button class="page-link" @click="handlePageChange(n)">{{ n }}</button>
            </li>
            <li class="page-item" :class="{ disabled: currentPage === totalPages }">
            <button class="page-link" @click="handlePageChange(currentPage + 1)">Next</button>
            </li>
        </ul>
    </nav>
</div>
</template>