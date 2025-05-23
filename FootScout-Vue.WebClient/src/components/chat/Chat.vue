<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, nextTick } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import { closeModal  } from '../../services/modal/ModalFunction';
import { AccountService } from '../../services/api/AccountService';
import ChatService from '../../services/api/ChatService';
import MessageService from '../../services/api/MessageService';
import ChatHub from '../../services/chat/ChatHub';
import { TimeService } from '../../services/time/TimeService';
import type { Chat } from '../../models/interfaces/Chat';
import type { Message } from '../../models/interfaces/Message';
import type { UserDTO } from '../../models/dtos/UserDTO';
import type { MessageSendDTO } from '../../models/dtos/MessageSendDTO';
import '../../styles/chat/Chat.css';

// Chat.vue - Komponent obsługujący funkcjonalność chatu między użytkownikami

const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
const route = useRoute();   // Pobranie informacji o aktualnej trasie (np. parametry w URL)
const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

// Inicjalizacja zmiennych przechowujących dane
const chatId = ref<number | null>(route.params.id ? Number(route.params.id) : null);
const userId = ref<string | null>(null);
const chatData = ref<Chat | null>(null);
const user = ref<UserDTO | null>(null);
const receiver = ref<UserDTO | null>(null);
const messages = ref<Message[]>([]);          // Lista wiadomości
const chatHub = ref<ChatHub | null>(null);    // ChatHub do obsługi połączenia WebSocket
const newMessage = ref<string>('');     // Nowa wiadomość
const messagesEndRef = ref<HTMLElement | null>(null);   // Referencja do końca listy wiadomości (do scrollowania)
const deleteMessageId = ref<number | null>(null);   // ID wiadomości do usunięcia

// Ładowanie danych użytkownika po załadowaniu komponentu
onMounted(async () => {
  await fetchUserData();
  if (chatId.value) {
    await fetchChatData(chatId.value);
  }
});

// Obserwacja zmian w ID czatu
watch(chatId, async (newChatId) => {
  if (newChatId) {
    await fetchChatData(newChatId);
  }
});

// Zamykanie połączenia z ChatHub przy unmountowaniu komponentu
onUnmounted(() => {
  if (chatHub.value) {
    chatHub.value.leaveChat().catch((error) => {
      console.error('Failed to leave chat:', error);
    });
  }
});

// Funkcja do pobierania danych użytkownika
async function fetchUserData() {
  try {
    // Pobranie ID użytkownika
    userId.value = await AccountService.getId();
  }
  catch (error) {
    console.error('Failed to fetch userId:', error);
    toast.error('Failed to load userId.');
  }
}

// Funkcja do pobierania danych czatu
async function fetchChatData(id: number) {
  try {
    const _chatData = await ChatService.getChatById(id);
    chatData.value = _chatData;

    // Określenie, kto jest nadawcą i odbiorcą wiadomości
    if (_chatData.user1Id === userId.value) {
      user.value = _chatData.user1;
      receiver.value = _chatData.user2;
    }
    else {
      user.value = _chatData.user2;
      receiver.value = _chatData.user1;
    }

    // Pobranie wiadomości dla czatu
    messages.value = await MessageService.getMessagesForChat(id);
    startChatService(id);
  }
  catch (error) {
    console.error('Failed to fetch chat data:', error);
    toast.error('Failed to load chat data.');
  }
}

// Funkcja do inicjalizacji połączenia z ChatHub
function startChatService(id: number) {
  if (!userId.value) return;

  // Inicjalizacja ChatHub do obsługi wiadomości w czasie rzeczywistym
  const _chatHub = new ChatHub((message) => {
    messages.value.push(message);   // Dodanie nowej wiadomości
    nextTick(scrollToBottom);       // Scrollowanie do dołu po dodaniu wiadomości
  });

  // Połączenie z ChatHub
  _chatHub
    .startConnection(id)
    .then(() => {
      chatHub.value = _chatHub;
    })
    .catch((error) => {
      console.error('Failed to start chat service:', error);
      toast.error('Failed to start chat service.');
    });
}

// Funkcja do scrollowania do ostatniej wiadomości
function scrollToBottom() {
  nextTick(() => {
    messagesEndRef.value?.scrollIntoView({ behavior: 'smooth' });
  });
}

// Funkcja do wysyłania wiadomości
async function handleSendMessage() {
  if (!chatHub.value || newMessage.value.trim() === '' || !chatData.value || !user.value || !receiver.value) {
    toast.error('Unable to send message. Missing required data.');
    return;
  }

  try {
    const messageSendDTO: MessageSendDTO = {
      chatId: chatData.value.id,
      senderId: user.value.id,
      receiverId: receiver.value.id,
      content: newMessage.value
    };

    await chatHub.value.sendMessage(messageSendDTO);    // Wysłanie wiadomości przez ChatHub
    newMessage.value = '';    // Wyczyść pole tekstowe
    scrollToBottom();

    // Pobranie zaktualizowanych wiadomości
    messages.value = await MessageService.getMessagesForChat(chatData.value.id);
  }
  catch (error) {
    console.error('Failed to send message:', error);
    toast.error('Failed to send message.');
  }
}

// Funkcja do usuwania całego czatu
async function handleDeleteChatRoom() {
  if (!chatData.value)
    return;

  try {
    await ChatService.deleteChat(chatData.value.id);
    closeModal('deleteChatRoomModal');
    toast.success('Your chat room has been deleted successfully.');
    router.push('/chats');    // Przekierowanie do listy czatów
  }
  catch (error) {
    console.error('Failed to delete chat room:', error);
    toast.error('Failed to delete chat room.');
  }
}

// Funkcja do ustawienia ID wiadomości do usunięcia
const handleDeleteMessage = (messageId: number) => {
  deleteMessageId.value = messageId;
};

// Funkcja do usuwania wiadomości
async function deleteMessage() {
  if (!deleteMessageId.value)
    return;

  try {
    await MessageService.deleteMessage(deleteMessageId.value);
    toast.success('Message has been deleted successfully.');

    // Zaktualizowanie listy wiadomości
    deleteMessageId.value = null;
    messages.value = await MessageService.getMessagesForChat(chatData.value?.id ?? 0);
    closeModal('deleteMessageModal');
  }
  catch (error) {
    console.error('Failed to delete message:', error);
    toast.error('Failed to delete message.');
  }
}

</script>
<!-- Struktura chatu: lista wiadomości, formularz wysyłania i modale usuwania -->
<template>
  <div class="Chat">
    <h1><i class="bi bi-chat-dots-fill"></i> Chat</h1>

    <div class="chat-container">
      <!-- Pasek nawigacyjny chatu -->
      <nav class="navbar navbar-dark bg-dark sticky-top">
        <div class="container d-flex justify-content-between align-items-center">
          <span class="navbar-brand mx-auto d-flex align-items-center">
            <i class="bi bi-person-circle me-2"></i>
            {{ receiver ? `${receiver.firstName} ${receiver.lastName}` : 'Receiver' }}
          </span>
          <button class="btn btn-danger btn-sm" data-bs-toggle="modal" data-bs-target="#deleteChatRoomModal">
            <i class="bi bi-trash"></i>
          </button>
        </div>
      </nav>

      <!-- wiadomości -->
      <div class="messages">
        <div v-if="messages.length > 0">
          <div v-for="(message, index) in messages" :key="index" class="row my-2">
            <div
              class="col-7"
              :class="{ 'offset-5': message.senderId === userId }"
            >
              <div class="d-flex justify-content-between align-items-center mb-1">
                <span>{{ message.sender ? `${message.sender.firstName} ${message.sender.lastName}` : 'Sender' }}</span>
                <small class="text-muted">{{ TimeService.formatDateToEURWithHour(message.timestamp) }}</small>
                <button
                  v-if="message.senderId === userId"
                  class="btn btn-secondary btn-sm"
                  data-bs-toggle="modal" data-bs-target="#deleteMessageModal" 
                  @click="handleDeleteMessage(message.id)"
                  >
                  <i class="bi bi-trash"></i>
                </button>
              </div>
              <div
                class="card"
                :class="message.senderId === userId ? 'bg-primary text-white' : 'bg-light'"
              >
                <div class="card-body">
                  <p class="card-text">{{ message.content }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div v-else class="text-center mt-4">
          <p><strong>You're starting a new conversation</strong></p>
          <p>Type your first message below.</p>
        </div>
        <div ref="messagesEndRef"></div>
      </div>

      <!-- Forms wysyłania wiadomości -->
      <form @submit.prevent="handleSendMessage" class="message-input">
        <div class="row">
          <div class="col-10">
            <input
              type="text"
              v-model="newMessage"
              class="form-control"
              placeholder="Type a message"
              @keypress.enter.prevent="handleSendMessage"
            />
          </div>
          <div class="col-2">
            <button type="submit" class="btn btn-dark w-100">
              <i class="bi bi-send-fill"></i>
            </button>
          </div>
        </div>
      </form>
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
                <button class="btn btn-secondary" @click="closeModal('deleteChatRoomModal')">Cancel</button>
                <button class="btn btn-danger" @click="handleDeleteChatRoom">Delete</button>
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
            <div class="modal-body">
                Are you sure you want to delete this message?
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" @click="closeModal('deleteMessageModal')">Cancel</button>
                <button class="btn btn-danger" @click="deleteMessage">Delete</button>
            </div>
            </div>
        </div>
      </div>
  </div>
</template>