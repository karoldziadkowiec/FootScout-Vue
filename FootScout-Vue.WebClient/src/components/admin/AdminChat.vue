<script setup lang="ts">
import { ref, onMounted, watch, nextTick } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useToast } from 'vue-toast-notification';
import { closeModal  } from '../../services/modal/ModalFunction';
import { TimeService } from '../../services/time/TimeService';
import ChatService from '../../services/api/ChatService';
import MessageService from '../../services/api/MessageService';
import type { Chat } from '../../models/interfaces/Chat';
import type { Message } from '../../models/interfaces/Message';
import type { UserDTO } from '../../models/dtos/UserDTO';
import '../../styles/admin/AdminChat.css';

const toast = useToast();
const route = useRoute();
const router = useRouter();

const id = ref<number | null>(route.params.id ? Number(route.params.id) : null);
const chatData = ref<Chat | null>(null);
const user1 = ref<UserDTO | null>(null);
const user2 = ref<UserDTO | null>(null);
const messages = ref<Message[]>([]);
const messagesCount = ref<number>(0);
const messagesEndRef = ref<HTMLElement | null>(null);
const deleteMessageId = ref<number | null>(null);

const fetchChatData = async () => {
  if (!id.value)
    return;
  try {
    const _chatData = await ChatService.getChatById(id.value);
    chatData.value = _chatData;
    user1.value = _chatData.user1;
    user2.value = _chatData.user2;

    const _messages = await MessageService.getMessagesForChat(id.value);
    messages.value = _messages;

    messagesCount.value = await MessageService.getMessagesForChatCount(id.value);
    scrollToBottom();
  }
  catch (error) {
    console.error('Failed to fetch chat data:', error);
    toast.error('Failed to load chat data.');
  }
};

// Automatyczne przewijanie do dołu
const scrollToBottom = async () => {
  await nextTick();
  messagesEndRef.value?.scrollIntoView({ behavior: 'smooth' });
};

// Odświeżanie wiadomości
const refreshData = async () => {
  if (!id.value)
    return;
  messages.value = await MessageService.getMessagesForChat(id.value);
  messagesCount.value = await MessageService.getMessagesForChatCount(id.value);
  scrollToBottom();
};

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

const handleDeleteMessageModal = (messageId: number) => {
    deleteMessageId.value = messageId;
};

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

// Pobierz dane przy montowaniu
onMounted(() => {
  if (id.value)
    fetchChatData();
});

watch(messages, () => {
  scrollToBottom();
});
</script>

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