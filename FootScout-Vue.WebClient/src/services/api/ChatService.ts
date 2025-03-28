import axios from 'axios';
import { ApiURL } from '../../config/ApiURL';
import { AccountService } from './AccountService';
import type { Chat } from '../../models/interfaces/Chat';
import type { ChatCreateDTO } from '../../models/dtos/ChatCreateDTO';

// ChatService.ts - Serwis do zarządzania czatami, obsługujący komunikację z API za pomocą axios.

const ChatService = {
    // Pobiera pojedynczy czat na podstawie jego ID
    async getChatById(chatId: number): Promise<Chat> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<Chat>(`${ApiURL}/chats/${chatId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching chat, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Pobiera listę wszystkich czatów
    async getChats(): Promise<Chat[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<Chat[]>(`${ApiURL}/chats`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching all chats, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Pobiera liczbę dostępnych czatów
    async getChatCount(): Promise<number> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<number>(`${ApiURL}/chats/count`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching chat count, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Pobiera ID czatu między dwoma użytkownikami
    async getChatIdBetweenUsers(user1Id: string, user2Id: string): Promise<number> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<number>(`${ApiURL}/chats/between/${user1Id}/${user2Id}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching chat id, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Tworzy nowy czat na podstawie przekazanych danych DTO
    async createChat(dto: ChatCreateDTO): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            await axios.post(`${ApiURL}/chats`, dto, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error creating new chat, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Usuwa czat na podstawie jego ID
    async deleteChat(chatId: number): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            await axios.delete(`${ApiURL}/chats/${chatId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error deleting chat, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Eksportuje wszystkie czaty do pliku CSV i pobiera go na urządzenie użytkownika
    async exportChatsToCsv(): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();

            const response = await axios.get(`${ApiURL}/chats/export`, {
                headers: {
                    'Authorization': authorizationHeader
                },
                responseType: 'blob'        // Pobieranie danych w postaci pliku binarnego
            });

            // Tworzy URL dla pobranego pliku i inicjalizuje pobieranie
            const url = window.URL.createObjectURL(new Blob([response.data], { type: 'text/csv' }));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', 'chats.csv');

            document.body.appendChild(link);
            link.click();       // Kliknięcie linku, aby rozpocząć pobieranie
            link.remove();      // Usunięcie elementu po pobraniu
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error("Error exporting chats to CSV, details:", error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    }
};

// Eksportuje serwis czatu, umożliwiając jego użycie w innych modułach.
export default ChatService;