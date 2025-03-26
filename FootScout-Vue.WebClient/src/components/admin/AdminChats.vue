<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import { closeModal  } from '../../services/modal/ModalFunction';
import { TimeService } from '../../services/time/TimeService';
import ChatService from '../../services/api/ChatService';
import MessageService from '../../services/api/MessageService';
import type { Chat } from '../../models/interfaces/Chat';
import '../../styles/admin/AdminChats.css';

// AdminChats.vue - Komponent zarządzający listą czatów administratora

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Zmienne do przechowywania chatów oraz dat ostatnich wiadomości i liczby wiadomości
const chatRooms = ref<Chat[]>([]);
const lastMessageDates = ref<Map<number, string>>(new Map());
const messagesCounters = ref<Map<number, number>>(new Map());
const deleteChatRoomId = ref<number | null>(null);

// Zmienne do wyszukiwania chatów oraz do sortowania
const searchTerm = ref<string>('');
const sortCriteria = ref<string>('lastMessageDesc');

// Zmienne do paginacji
const currentPage = ref<number>(1);
const itemsPerPage = 20;
const indexOfLastItem = computed(() => currentPage.value * itemsPerPage);
const indexOfFirstItem = computed(() => indexOfLastItem.value - itemsPerPage);

// Funkcja pobierająca czaty
const fetchChats = async () => {
  try {
    chatRooms.value = await ChatService.getChats();
    await fetchDataForSpecificChatRoom(chatRooms.value);
  }
  catch (error) {
    console.error('Failed to fetch chats:', error);
    toast.error('Failed to load chats.');
  }
};

// Funkcja pobierająca dodatkowe dane dla każdego czatu (daty ostatnich wiadomości i liczba wiadomości)
const fetchDataForSpecificChatRoom = async (chats: Chat[]) => {
  const dates = new Map<number, string>();
  const counters = new Map<number, number>();

  for (const chat of chats) {
    try {
      const date = await MessageService.getLastMessageDateForChat(chat.id);
      dates.set(chat.id, date === '0001-01-01T00:00:00' ? '-' : date);

      const counter = await MessageService.getMessagesForChatCount(chat.id);
      counters.set(chat.id, counter);
    }
    catch (error) {
      console.error(`Failed to fetch last message date for chat ${chat.id}:`, error);
    }
  }
  lastMessageDates.value = dates;
  messagesCounters.value = counters;
};

// Funkcja zmieniająca stronę
const handlePageChange = (pageNumber: number) => {
  currentPage.value = pageNumber;
};

// Funkcja przechodząca do konkretnego czatu
const moveToSpecificChatPage = (chatId: number) => {
  router.push({ path: `/admin/chat/${chatId}`, state: { chatId } });
};

// Funkcja otwierająca modal do usuwania chat roomu
const handleShowDeleteChatRoomModal = (chatRoomId: number) => {
  deleteChatRoomId.value = chatRoomId;
};

// Funkcja usuwająca chat room
const deleteChatRoom = async () => {
  if (!deleteChatRoomId.value)
    return;

  try {
    await ChatService.deleteChat(deleteChatRoomId.value);
    toast.success('Chat room has been deleted successfully.');
    deleteChatRoomId.value = null;
    await fetchChats();
    closeModal('deleteChatRoomModal')
  }
  catch (error) {
    console.error('Failed to delete chat room:', error);
    toast.error('Failed to delete chat room.');
  }
};

// Funkcja do filtrowania czatów na podstawie wyszukiwanego terminu
const searchChats = (chats: Chat[]): Chat[] => {
    if (!searchTerm) {
        return chats;
    }
    const lowerCaseSearchTerm = searchTerm.value.toLowerCase();
    return chats.filter(chat =>
        (chat.user1.firstName + ' ' + chat.user1.lastName).toLowerCase().includes(lowerCaseSearchTerm) ||
        chat.user1.email.toLowerCase().includes(lowerCaseSearchTerm) ||
        (chat.user2.firstName + ' ' + chat.user2.lastName).toLowerCase().includes(lowerCaseSearchTerm) ||
        chat.user2.email.toLowerCase().includes(lowerCaseSearchTerm)
    );
};

// Funkcja do parsowania daty
const parseDate = (dateString: string | undefined) => {
    if (!dateString) return new Date('1970-01-01').getTime(); // Domyślna wartość dla pustych dat
    const parsedDate = new Date(dateString);
    return isNaN(parsedDate.getTime()) ? new Date('1970-01-01').getTime() : parsedDate.getTime();
};

// Funkcja do sortowania czatów na podstawie wybranego kryterium
const sortChats = (chats: Chat[], lastMessageDates: Map<number, string>, messagesCounters: Map<number, number>) => {
    return [...chats].sort((a, b) => {
        const lastMessageDateA = parseDate(lastMessageDates.get(a.id));
        const lastMessageDateB = parseDate(lastMessageDates.get(b.id));

        const messagesCounterA = messagesCounters.get(a.id) || 0;
        const messagesCounterB = messagesCounters.get(b.id) || 0;

        switch (sortCriteria.value) {
          // Sortowanie po dacie ostatniej wiadomości rosnąco
            case 'lastMessageAsc': 
                return lastMessageDateA - lastMessageDateB;
          // Sortowanie po dacie ostatniej wiadomości malejąco
            case 'lastMessageDesc': 
                return lastMessageDateB - lastMessageDateA;
          // Sortowanie po liczbie wiadomości rosnąco
            case 'messagesCounterAsc': 
                return messagesCounterA - messagesCounterB;
          // Sortowanie po liczbie wiadomości malejąco
            case 'messagesCounterDesc': 
                return messagesCounterB - messagesCounterA;
            default:
                return 0;
        }
    });
};

// Pobieranie danych przy montowaniu komponentu
onMounted(async () => {
  await fetchChats();
});

// Paginacja
// Paginacja: przefiltrowane czaty na podstawie wyszukiwania
const searchedChats = computed(() => searchChats(chatRooms.value));
// Paginacja: posortowane czaty na podstawie wybranego kryterium
const sortedChats = computed(() => sortChats(searchedChats.value, lastMessageDates.value, messagesCounters.value));
// Paginacja: elementy czatów, które mają być wyświetlane na bieżącej stronie
const currentChatItems = computed(() => sortedChats.value.slice(indexOfFirstItem.value, indexOfLastItem.value));
// Paginacja: obliczanie liczby stron
const totalPages = computed(() => Math.ceil(sortedChats.value.length / itemsPerPage));

</script>
<!-- Struktura strony admina: lista czatów i szczegóły -->
<template>
  <div class="AdminChats">
    <h1><i class="bi bi-chat-text-fill"></i> Chat Rooms</h1>
    <p></p>
    <div class="d-flex align-items-center mb-3">
      <div>
        <label class="form-label"><strong>Search</strong></label>
        <input type="text" class="form-control" placeholder="Search" v-model="searchTerm" />
      </div>

      <div class="ms-auto">
        <label class="form-label"><strong>Sort by</strong></label>
        <select class="form-select" v-model="sortCriteria">
          <option value="lastMessageAsc">Last Message Ascending</option>
          <option value="lastMessageDesc">Last Message Descending</option>
          <option value="messagesCounterAsc">Messages Counter Ascending</option>
          <option value="messagesCounterDesc">Messages Counter Descending</option>
        </select>
      </div>
    </div>

    <div class="table-responsive">
      <table class="table table-striped table-bordered table-hover">
        <thead class="table-dark">
          <tr>
            <th>User 1</th>
            <th>User 2</th>
            <th>Last message</th>
            <th>Messages count</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="chat in currentChatItems" :key="chat.id">
            <td>{{ chat.user1.firstName }} {{ chat.user1.lastName }} ({{ chat.user1.email }})</td>
            <td>{{ chat.user2.firstName }} {{ chat.user2.lastName }} ({{ chat.user2.email }})</td>
            <td>{{ TimeService.formatDateToEURWithHour(lastMessageDates.get(chat.id) || '') || 'No messages' }}</td>
            <td>{{ messagesCounters.get(chat.id) || 0 }}</td>
            <td>
              <button class="btn btn-info me-2" @click="moveToSpecificChatPage(chat.id)">
                <i class="bi bi-chat-fill"></i>
              </button>
              <button class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#deleteChatRoomModal" @click="handleShowDeleteChatRoomModal(chat.id)">
                <i class="bi bi-trash"></i>
              </button>
            </td>
          </tr>
          <tr v-if="currentChatItems.length === 0">
            <td colspan="5" class="text-center">No chat room available</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal usuwania chat roomu -->
    <div class="modal" id="deleteChatRoomModal" tabindex="-1">
        <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
            <h5 class="modal-title">Confirm action</h5>
            <button type="button" class="btn-close" @click="closeModal('deleteChatRoomModal')"></button>
            </div>
            <div class="modal-body">
            Are you sure you want to delete this chat room?
            </div>
            <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeModal('deleteChatRoomModal')">Cancel</button>
            <button type="button" class="btn btn-danger" @click="deleteChatRoom">Delete</button>
            </div>
        </div>
        </div>
    </div>

    <!-- Paginacja -->
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