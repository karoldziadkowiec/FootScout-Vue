<script setup lang="ts">  
import { ref, onMounted, watch, nextTick } from 'vue';    // Import funkcji Vue do zarządzania stanem i cyklem życia komponentu
import { useRoute, useRouter } from 'vue-router';   // Importowanie routera Vue do obsługi nawigacji między stronami
import { useToast } from 'vue-toast-notification';    // Import systemu powiadomień toast do wyświetlania komunikatów użytkownikowi
import { closeModal  } from '../../services/modal/ModalFunction';   // Funkcja zamykająca modalne okna dialogowe
import { TimeService } from '../../services/time/TimeService';    // Serwis do operacji na datach i czasie
import ChatService from '../../services/api/ChatService';   // Serwis do obsługi API czatu (pobieranie, wysyłanie wiadomości)
import MessageService from '../../services/api/MessageService';   // Serwis do obsługi API wiadomości czatu
import type { Chat } from '../../models/interfaces/Chat';   // Typowanie dla obiektu czatu
import type { Message } from '../../models/interfaces/Message';   // Typowanie dla obiektu wiadomości
import type { UserDTO } from '../../models/dtos/UserDTO';   // Typowanie dla obiektu użytkownika DTO (Data Transfer Object)
import '../../styles/admin/AdminChat.css';    // Stylowanie dla panelu administratora czatu

// AdminChat.vue - Komponent zarządzający czatem administratora

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const route = useRoute();   // Pobranie informacji o aktualnej trasie (np. parametry w URL)
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Ref do przechowywania danych chatów i wiadomości
const id = ref<number | null>(route.params.id ? Number(route.params.id) : null);
const chatData = ref<Chat | null>(null);
const user1 = ref<UserDTO | null>(null);
const user2 = ref<UserDTO | null>(null);
const messages = ref<Message[]>([]);
const messagesCount = ref<number>(0);
const messagesEndRef = ref<HTMLElement | null>(null);
const deleteMessageId = ref<number | null>(null);

// Funkcja fetchChatData odpowiedzialna za pobranie danych o czacie i wiadomościach
const fetchChatData = async () => {
  if (!id.value)
    return;
  try {
    // Pobieranie danych czatu
    const _chatData = await ChatService.getChatById(id.value);
    chatData.value = _chatData;
    user1.value = _chatData.user1;
    user2.value = _chatData.user2;

    // Pobieranie wiadomości
    const _messages = await MessageService.getMessagesForChat(id.value);
    messages.value = _messages;

    // Liczba wiadomości
    messagesCount.value = await MessageService.getMessagesForChatCount(id.value);
     // Automatyczne przewijanie do ostatniej wiadomości
    scrollToBottom();
  }
  catch (error) {
    console.error('Failed to fetch chat data:', error);
    toast.error('Failed to load chat data.');
  }
};

// Funkcja odpowiedzialna za automatyczne przewijanie do dołu
const scrollToBottom = async () => {
  await nextTick();
  messagesEndRef.value?.scrollIntoView({ behavior: 'smooth' });
};

// Funkcja odświeżająca dane (nowe wiadomości)
const refreshData = async () => {
  if (!id.value)
    return;
  messages.value = await MessageService.getMessagesForChat(id.value);
  messagesCount.value = await MessageService.getMessagesForChatCount(id.value);
  scrollToBottom();
};

// Funkcja usuwająca chat
const deleteChatRoom = async () => {
  if (!chatData.value) return;
  try {
    await ChatService.deleteChat(chatData.value.id);
    router.push({ path: '/admin/chats', state: { toastMessage: 'Chat room has been deleted successfully.' } });
    closeModal('deleteChatRoomModal');
  }
  catch (error) {
    console.error('Failed to delete chat room:', error);
    toast.error('Failed to delete chat room.');
  }
};

// Funkcja obsługująca otwieranie modala do usuwania wiadomości
const handleDeleteMessageModal = (messageId: number) => {
    deleteMessageId.value = messageId;
};

// Funkcja usuwająca wiadomość
const deleteMessage = async () => {
  if (!deleteMessageId.value)
    return;
  try {
    await MessageService.deleteMessage(deleteMessageId.value);
    toast.success('Message has been deleted successfully.');
    deleteMessageId.value = null;
    refreshData();
    closeModal('deleteMessageModal');
  }
  catch (error) {
    console.error('Failed to delete message:', error);
    toast.error('Failed to delete message.');
  }
};

// Pobranie danych po załadowaniu komponentu
onMounted(() => {
  if (id.value)
    fetchChatData();
});

// Obserwator, który przewija ekran na dół po zmianie wiadomości
watch(messages, () => {
  scrollToBottom();
});

</script>
<!-- Struktura strony: zarządzanie szczegółami wybranego czatu administratora-->
<template>
  <div class="AdminChat">
    <h1><i class="bi bi-chat-dots"></i> Manage Chat</h1>
    <h4>Messages count: <strong>{{ messagesCount }}</strong></h4>

    <div class="chat-container">
      <nav class="navbar navbar-dark bg-dark sticky-top">
        <div class="container">
          <span class="navbar-brand mx-auto chat-name d-flex align-items-center">
            <span v-if="user1 && user2">
              {{ user1.firstName }} {{ user1.lastName }} - {{ user2.firstName }} {{ user2.lastName }}
            </span>
            <div class="ms-auto">
              <button class="btn btn-info btn-sm me-2" @click="refreshData">
                <i class="bi bi-arrow-repeat"></i>
              </button>
              <button class="btn btn-danger btn-sm" data-bs-toggle="modal" data-bs-target="#deleteChatRoomModal">
                <i class="bi bi-trash"></i>
              </button>
            </div>
          </span>
        </div>
      </nav>

      <div class="messages">
        <template v-if="messages.length > 0">
          <div v-for="(message, index) in messages" :key="index" class="row my-2">
            <div
              class="col-12 col-sm-7"
              :class="{ 'offset-sm-5': message.senderId === user2?.id }"
            >
              <div class="d-flex justify-content-between align-items-center">
                <span>
                  {{ message.sender ? `${message.sender.firstName} ${message.sender.lastName}` : 'Sender' }}
                </span>
                <span class="message-timestamp">
                  {{ TimeService.formatDateToEURWithHour(message.timestamp) }}
                </span>
                <button class="btn btn-secondary btn-sm" data-bs-toggle="modal" data-bs-target="#deleteMessageModal" @click="handleDeleteMessageModal(message.id)">
                  <i class="bi bi-trash"></i>
                </button>
              </div>

              <div class="card" :class="{ 'bg-primary text-white': message.senderId === user2?.id, 'bg-light': message.senderId !== user2?.id }">
                <div class="card-body">
                  <p class="card-text">{{ message.content }}</p>
                </div>
              </div>
            </div>
          </div>
        </template>
        <div v-else>
          <p><strong>It's a new conversation</strong></p>
          <p>Waiting for first message...</p>
        </div>
        <div ref="messagesEndRef"></div>
      </div>
    </div>

    <!-- Modal usuwania chat roomu -->
    <div class="modal" id="deleteChatRoomModal" tabindex="-1">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Confirm action</h5>
            <button type="button" class="btn-close" @click="closeModal('deleteChatRoomModal')"></button>
          </div>
          <div class="modal-body">Are you sure you want to delete this chat room?</div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="closeModal('deleteChatRoomModal')">Cancel</button>
            <button class="btn btn-danger" @click="deleteChatRoom">Delete</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal usuwania wiadomości -->
    <div class="modal" id="deleteMessageModal" tabindex="-1">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Confirm action</h5>
            <button type="button" class="btn-close" @click="closeModal('deleteMessageModal')"></button>
          </div>
          <div class="modal-body">Are you sure you want to delete this message?</div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="closeModal('deleteMessageModal')">Cancel</button>
            <button class="btn btn-danger" @click="deleteMessage">Delete</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>