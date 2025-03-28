import axios from 'axios';
import { ApiURL } from '../../config/ApiURL';
import { AccountService } from './AccountService';
import type { Message } from '../../models/interfaces/Message';

// Serwis do zarządzania wiadomomściami czatów, wykorzystujący axios do komunikacji z API

const MessageService = {
    // Funkcja do pobrania wszystkich wiadomości
    async getAllMessages(): Promise<Message[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wykonywanie zapytania GET do API, by pobrać wszystkie wiadomości
            const response = await axios.get<Message[]>(`${ApiURL}/messages`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching all messages, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;        // Rzucanie błędem, by inne części aplikacji mogły go obsłużyć
        }
    },

    // Funkcja do pobrania liczby wszystkich wiadomości
    async getAllMessagesCount(): Promise<number> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<number>(`${ApiURL}/messages/count`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching messages count, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja do pobrania wiadomości dla konkretnego czatu (na podstawie chatId)
    async getMessagesForChat(chatId: number): Promise<Message[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<Message[]>(`${ApiURL}/messages/chat/${chatId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching messages for chat, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja do pobrania liczby wiadomości dla konkretnego czatu
    async getMessagesForChatCount(chatId: number): Promise<number> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<number>(`${ApiURL}/messages/chat/${chatId}/count`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching messages for chat count, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja do pobrania daty ostatniej wiadomości dla konkretnego czatu
    async getLastMessageDateForChat(chatId: number): Promise<string> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<string>(`${ApiURL}/messages/chat/${chatId}/last-message-date`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching last message date, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja do usunięcia wiadomości na podstawie jej identyfikatora
    async deleteMessage(messageId: number): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            await axios.delete(`${ApiURL}/messages/${messageId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error deleting message, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    }
};

// Eksportowanie serwisu, by móc używać go w innych częściach aplikacji
export default MessageService;