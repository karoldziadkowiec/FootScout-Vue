<script setup lang="ts">
import { ref, onMounted, computed  } from "vue";
import { useRouter } from "vue-router";
import { useToast } from 'vue-toast-notification';
import { closeModal  } from '../../services/modal/ModalFunction';
import { TimeService } from '../../services/time/TimeService';
import { AccountService } from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import ChatService from '../../services/api/ChatService';
import type { UserDTO } from '../../models/dtos/UserDTO';
import type { ChatCreateDTO } from '../../models/dtos/ChatCreateDTO';
import '../../styles/admin/AdminMakeAnAdmin.css';

const toast = useToast();
const router = useRouter();

const userId = ref<string | null>(null);
const onlyUsers = ref<UserDTO[]>([]);
const onlyAdmins = ref<UserDTO[]>([]);
const userIdToMakeAdmin = ref<string | null>(null);
const userIdToMakeUser = ref<string | null>(null);
const searchTerm = ref<string>("");
const currentPage = ref<number>(1);
const itemsPerPage = 20;
const indexOfLastItem = computed(() => currentPage.value * itemsPerPage);
const indexOfFirstItem = computed(() => indexOfLastItem.value - itemsPerPage);
const activeTab = ref("users");

onMounted(async () => {
  await fetchUserData();
  await fetchUsers();
});

const fetchUserData = async () => {
  try {
    userId.value = await AccountService.getId();
  }
  catch (error) {
    console.error("Failed to fetch user's data:", error);
    toast.error("Failed to load user's data.");
  }
};

const fetchUsers = async () => {
  try {
    onlyUsers.value = await UserService.getOnlyUsers();
    onlyAdmins.value = await UserService.getOnlyAdmins();
  }
  catch (error) {
    console.error("Failed to fetch users:", error);
    toast.error("Failed to load users.");
  }
};

const handlePageChange = (page: number) => {
  currentPage.value = page;
};

const handleShowMakeAnAdminModal = (id: string) => {
  userIdToMakeAdmin.value = id;
};

const makeAnAdmin = async () => {
  if (!userIdToMakeAdmin.value)
    return;

  try {
    await AccountService.makeAnAdmin(userIdToMakeAdmin.value);
    toast.success("Admin permissions granted.");
    await fetchUsers();
    closeModal('makeAnAdminModal');
  }
  catch (error) {
    console.error("Failed to grant admin permissions:", error);
    toast.error("Failed to grant admin permissions.");
  }
};

const handleShowMakeAnUserModal = (id: string) => {
  userIdToMakeUser.value = id;
};

const makeAnUser = async () => {
  if (!userIdToMakeUser.value) return;

  try {
    await AccountService.makeAnUser(userIdToMakeUser.value);
    toast.success("Admin permissions revoked.");
    await fetchUsers();
    closeModal('makeAnUserModal');
  }
  catch (error) {
    console.error("Failed to revoke admin permissions:", error);
    toast.error("Failed to revoke admin permissions.");
  }
};

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

const searchUsers = (users: UserDTO[]) => {
    if (!searchTerm.value) {
        return users;
    }
    const lowerCaseSearchTerm = searchTerm.value.toLowerCase();
    return users.filter(user =>
        (user.firstName + ' ' + user.lastName).toLowerCase().includes(lowerCaseSearchTerm) ||
        user.email.toLowerCase().includes(lowerCaseSearchTerm)
    );
};

const searchedUsers = computed(() => searchUsers(onlyUsers.value));
const currentUserItems = computed(() => searchedUsers.value.slice(indexOfFirstItem.value, indexOfLastItem.value));
const totalPages = computed(() => Math.ceil(searchedUsers.value.length / itemsPerPage));
</script>

<template>
    <div class="AdminMakeAnAdmin">
        <h1><i class="bi bi-universal-access-circle"></i> Make an Admin</h1>

        <!-- Zakładki -->
        <div class="mb-3">
            <ul class="nav nav-tabs">
                <li class="nav-item">
                    <button class="nav-link" :class="{ active: activeTab === 'users' }" @click="activeTab = 'users'">Users</button>
                </li>
                <li class="nav-item">
                    <button class="nav-link" :class="{ active: activeTab === 'admins' }" @click="activeTab = 'admins'">Admins</button>
                </li>
            </ul>
        </div>

        <!-- Tabela użytkowników -->
        <div v-if="activeTab === 'users'">
            <div class="d-flex align-items-center mb-3">
                <div class="mx-auto">
                    <label class="form-label"><strong>Search</strong></label>
                    <input type="text" class="form-control" placeholder="Search" v-model="searchTerm" />
                </div>
            </div>
            <div class="table-responsive">
                <table class="table table-striped table-bordered table-hover">
                    <thead class="table-dark">
                        <tr>
                            <th>Creation Date</th>
                            <th>E-mail</th>
                            <th>First Name</th>
                            <th>Last Name</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-if="currentUserItems.length > 0" v-for="(user, index) in currentUserItems" :key="index">
                            <td>{{ TimeService.formatDateToEURWithHour(user.creationDate) }}</td>
                            <td>{{ user.email }}</td>
                            <td>{{ user.firstName }}</td>
                            <td>{{ user.lastName }}</td>
                            <td>
                                <button class="btn btn-warning me-2" data-bs-toggle="modal" data-bs-target="#makeAnAdminModal" @click="handleShowMakeAnAdminModal(user.id)">
                                    <i class="bi bi-universal-access-circle"></i>
                                </button>
                                <button v-if="user.id !== userId" class="btn btn-info" @click="handleOpenChat(user.id)">
                                    <i class="bi bi-chat-fill"></i>
                                </button>
                            </td>
                        </tr>
                        <tr v-else>
                            <td colspan="6" class="text-center">No user available</td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <!-- Paginacja -->
            <div class="admin-pagination-container">
                <nav>
                    <ul class="pagination pagination-blue">
                        <li class="page-item" :class="{ disabled: currentPage === 1 }">
                            <button class="page-link" @click="handlePageChange(currentPage - 1)">«</button>
                        </li>
                        <li class="page-item" v-for="page in totalPages" :key="page" :class="{ active: page === currentPage }">
                            <button class="page-link" @click="handlePageChange(page)">{{ page }}</button>
                        </li>
                        <li class="page-item" :class="{ disabled: currentPage === totalPages }">
                            <button class="page-link" @click="handlePageChange(currentPage + 1)">»</button>
                        </li>
                    </ul>
                </nav>
            </div>
        </div>

        <!-- Tabela adminów -->
        <div v-if="activeTab === 'admins'">
            <div class="table-responsive">
                <table class="table table-striped table-bordered table-hover">
                    <thead class="table-dark">
                        <tr>
                            <th>Creation Date</th>
                            <th>E-mail</th>
                            <th>First Name</th>
                            <th>Last Name</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-if="onlyAdmins.length > 0" v-for="(admin, index) in onlyAdmins" :key="index">
                            <td>{{ TimeService.formatDateToEURWithHour(admin.creationDate) }}</td>
                            <td>{{ admin.email }}</td>
                            <td>{{ admin.firstName }}</td>
                            <td>{{ admin.lastName }}</td>
                            <td>
                                <button v-if="admin.id !== userId" class="btn btn-danger me-2" data-bs-toggle="modal" data-bs-target="#makeAnUserModal" @click="handleShowMakeAnUserModal(admin.id)">
                                    <i class="bi bi-universal-access-circle"></i>
                                </button>
                                <button v-if="admin.id !== userId" class="btn btn-info" @click="handleOpenChat(admin.id)">
                                    <i class="bi bi-chat-fill"></i>
                                </button>
                            </td>
                        </tr>
                        <tr v-else>
                            <td colspan="6" class="text-center">No admin available</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <!-- Modal: Make an Admin -->
        <div class="modal" id="makeAnAdminModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Confirm action</h5>
                        <button type="button" class="btn-close" @click="closeModal('makeAnAdminModal')"></button>
                    </div>
                    <div class="modal-body">
                        Are you sure you want to grant admin permissions to this user?
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" @click="closeModal('makeAnAdminModal')">Cancel</button>
                        <button type="button" class="btn btn-primary" @click="makeAnAdmin">Accept</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal: Make a User -->
        <div class="modal" id="makeAnUserModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Confirm action</h5>
                        <button type="button" class="btn-close" @click="closeModal('makeAnUserModal')"></button>
                    </div>
                    <div class="modal-body">
                        Are you sure you want to demote admin permissions from this user?
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" @click="closeModal('makeAnUserModal')">Cancel</button>
                        <button type="button" class="btn btn-primary" @click="makeAnUser">Accept</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>