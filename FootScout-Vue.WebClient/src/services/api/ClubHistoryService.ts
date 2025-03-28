import axios from 'axios';
import { ApiURL } from '../../config/ApiURL';
import { AccountService } from './AccountService';
import type { ClubHistoryModel } from '../../models/interfaces/ClubHistory';
import type { ClubHistoryCreateDTO } from '../../models/dtos/ClubHistoryCreateDTO';

// Serwis do zarządzania historiami klubowymi użytkowników, wykorzystujący axios do komunikacji z API
// Umożliwia wykonywanie operacji CRUD na zasobach historii klubowych

const ClubHistoryService = {
    // Pobiera historię klubową na podstawie ID
    async getClubHistory(clubHistoryId: number): Promise<ClubHistoryModel> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();      // Pobranie nagłówka autoryzacji
            const response = await axios.get<ClubHistoryModel>(`${ApiURL}/club-history/${clubHistoryId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;       // Zwrócenie pobranej historii
        }
        catch (error) {
            // Obsługa błędów - logowanie błędu w konsoli
            if (axios.isAxiosError(error)) {
                console.error('Error fetching club history, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;        // Przekazanie błędu dalej
        }
    },

    // Pobiera wszystkie historie klubowe
    async getAllClubHistory(): Promise<ClubHistoryModel[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<ClubHistoryModel[]>(`${ApiURL}/club-history`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching all club histories, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Pobiera liczbę dostępnych historii klubowych
    async getClubHistoryCount(): Promise<number> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<number>(`${ApiURL}/club-history/count`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching club history count, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Tworzy nową historię klubową
    async createClubHistory(clubHistory: ClubHistoryCreateDTO): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            await axios.post(`${ApiURL}/club-history`, clubHistory, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error creating new club history, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Aktualizuje istniejącą historię klubową
    async updateClubHistory(clubHistoryId: number, clubHistory: ClubHistoryModel): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            await axios.put(`${ApiURL}/club-history/${clubHistoryId}`, clubHistory, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error updating club history, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Usuwa historię klubową na podstawie ID
    async deleteClubHistory(clubHistoryId: number): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            await axios.delete(`${ApiURL}/club-history/${clubHistoryId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error deleting club history, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    }
};

// Eksportuje serwis historii klubu, umożliwiając jego użycie w innych plikach aplikacji.
export default ClubHistoryService;