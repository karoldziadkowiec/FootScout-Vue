<script setup lang="ts">
  import { ref, onMounted } from 'vue';
  import { useRouter, useRoute } from 'vue-router';
  import { useToast } from 'vue-toast-notification';
  import { closeModal  } from '../../services/modal/ModalFunction';
  import { AccountService } from '../../services/api/AccountService';
  import UserService from '../../services/api/UserService';
  import { TimeService } from '../../services/time/TimeService';
  import ChatService from '../../services/api/ChatService';
  import MessageService from '../../services/api/MessageService';
  import type { Chat } from '../../models/interfaces/Chat';
  import '../../styles/chat/Chats.css';
  
  // Chats.vue - Komponent zarządzający listą czatów użytkownika

  const router = useRouter(); // Pobranie instancji routera, umożliwia nawigację między stronami
  const route = useRoute();   // Pobranie informacji o aktualnej trasie (np. parametry w URL)
  const toast = useToast();   // Pobranie instancji systemu powiadomień (do wyświetlania komunikatów użytkownikowi)

  // Definicja zmiennych reaktywnych
  const userId = ref<string | null>(null);    // Przechowywanie ID użytkownika
  const userChats = ref<Chat[]>([]);          // Lista czatów użytkownika
  const lastMessageDates = ref(new Map<number, string>());    // Mapowanie ID czatu na datę ostatniej wiadomości
  const deleteChatRoomId = ref<number | null>(null);      // ID czatu do usunięcia
  
  // Pobranie listy czatów użytkownika po zamontowaniu komponentu
  onMounted(async () => {
    if (route.query.toastMessage) {
      toast.success(route.query.toastMessage as string);
    }
  
    try {
      // Pobranie ID zalogowanego użytkownika
      userId.value = await AccountService.getId();    
      if (userId.value) {
        // Pobranie czatów użytkownika
        const chats = await UserService.getUserChats(userId.value);
        userChats.value = chats;
        // Pobranie dat ostatnich wiadomości
        await fetchLastMessageDates(chats);
      }
    } catch (error) {
      console.error("Failed to fetch user's chats:", error);
      toast.error("Failed to load user's chats.");
    }
  });
  
  // Pobiera daty ostatnich wiadomości dla każdego czatu
  const fetchLastMessageDates = async (chats: Chat[]) => {
    const dates = new Map<number, string>();
    for (const chat of chats) {
      try {
        const date = await MessageService.getLastMessageDateForChat(chat.id);
        dates.set(chat.id, date === '0001-01-01T00:00:00' ? '-' : date);
      } catch (error) {
        console.error(`Failed to fetch last message date for chat ${chat.id}:`, error);
      }
    }
    lastMessageDates.value = dates;
  };
  
  // Przenosi użytkownika do konkretnego czatu
  const moveToSpecificChatPage = (chatId: number) => {
    router.push(`/chat/${chatId}`);
    };
  
  // Ustawia ID czatu do usunięcia i wyświetla modal
  const handleShowDeleteChatRoomModal = (chatRoomId: number) => {
    deleteChatRoomId.value = chatRoomId;
  };
  
  // Usuwa wybrany czat, odświeża listę czatów i zamyka modal
  const handleDeleteChatRoom = async () => {
    if (!userId.value || !deleteChatRoomId.value)
        return;
  
    try {
      await ChatService.deleteChat(deleteChatRoomId.value);
      toast.success("Your chat room has been deleted successfully.");
  
      // Ponowne pobranie listy czatów po usunięciu
      const chats = await UserService.getUserChats(userId.value);
      userChats.value = chats;
      await fetchLastMessageDates(chats);

      closeModal('deleteChatRoomModal');
    }
    catch (error) {
      console.error("Failed to delete chat room:", error);
      toast.error("Failed to delete chat room.");
    }
  };

</script>
<!-- Struktura strony: lista czatów użytkownika i szczegóły -->
<template>
    <div class="Chats mt-4">
        <h1><i class="bi bi-chat-fill"></i> Chat Rooms</h1>
        <p></p>
        
        <div class="table-responsive">
        <table class="table table-striped table-bordered table-hover">
            <thead class="table-dark">
            <tr>
                <th>User</th>
                <th>Last message</th>
                <th></th>
            </tr>
            </thead>
            <tbody>
            <tr v-if="userChats.length === 0">
                <td colspan="3" class="text-center">No chat room available</td>
            </tr>
            <tr v-for="chat in userChats" :key="chat.id">
                <td class="chat-room-row">
                {{ chat.user1Id === userId ? `${chat.user2.firstName} ${chat.user2.lastName}` : `${chat.user1.firstName} ${chat.user1.lastName}` }}
                </td>
                <td class="chat-room-row">
                {{ TimeService.formatDateToEURWithHour(lastMessageDates.get(chat.id) || '') || 'No messages' }}
                </td>
                <td class="chat-room-row">
                <button class="btn btn-info me-2" @click="moveToSpecificChatPage(chat.id)">
                    <i class="bi bi-chat-fill"></i>
                </button>
                <button class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#deleteChatRoomModal" @click="handleShowDeleteChatRoomModal(chat.id)">
                    <i class="bi bi-trash"></i>
                </button>
                </td>
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
                <button class="btn btn-secondary" @click="closeModal('deleteChatRoomModal')">Cancel</button>
                <button class="btn btn-danger" @click="handleDeleteChatRoom">Delete</button>
            </div>
            </div>
        </div>
        </div>
    </div>
</template>