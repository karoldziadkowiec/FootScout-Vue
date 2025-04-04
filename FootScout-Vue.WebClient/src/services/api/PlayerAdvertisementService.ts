import axios from 'axios';
import { ApiURL } from '../../config/ApiURL';
import { AccountService } from './AccountService';
import type { PlayerAdvertisement } from '../../models/interfaces/PlayerAdvertisement';
import type { PlayerAdvertisementCreateDTO } from '../../models/dtos/PlayerAdvertisementCreateDTO';

// Serwis do zarządzania ogłoszeniami piłkarskimi, wykorzystujący axios do komunikacji z API

const PlayerAdvertisementService = {
    // Funkcja do pobrania konkretnego ogłoszenia piłkarskiego
    async getPlayerAdvertisement(playerAdvertisementId: number): Promise<PlayerAdvertisement> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wykonujemy zapytanie GET do API, aby pobrać ogłoszenie o podanym ID
            const response = await axios.get<PlayerAdvertisement>(`${ApiURL}/player-advertisements/${playerAdvertisementId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching player advertisement, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja do pobrania wszystkich ogłoszeń piłkarskich
    async getAllPlayerAdvertisements(): Promise<PlayerAdvertisement[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<PlayerAdvertisement[]>(`${ApiURL}/player-advertisements`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching all player advertisements, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja do pobrania tylko aktywnych ogłoszeń piłkarskich
    async getActivePlayerAdvertisements(): Promise<PlayerAdvertisement[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<PlayerAdvertisement[]>(`${ApiURL}/player-advertisements/active`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching all active player advertisements, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja do pobrania liczby aktywnych ogłoszeń
    async getActivePlayerAdvertisementCount(): Promise<number> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<number>(`${ApiURL}/player-advertisements/active/count`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching active player advertiement count, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja do pobrania nieaktywnych ogłoszeń piłkarskich
    async getInactivePlayerAdvertisements(): Promise<PlayerAdvertisement[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<PlayerAdvertisement[]>(`${ApiURL}/player-advertisements/inactive`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching all inactive player advertisements, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja do tworzenia nowego ogłoszenia piłkarskiego
    async createPlayerAdvertisement(dto: PlayerAdvertisementCreateDTO): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wykonujemy zapytanie POST, aby utworzyć nowe ogłoszenie
            await axios.post(`${ApiURL}/player-advertisements`, dto, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error creating new player advertisement, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja do aktualizacji istniejącego ogłoszenia piłkarskiego
    async updatePlayerAdvertisement(playerAdvertisementId: number, playerAdvertisement: PlayerAdvertisement): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wykonujemy zapytanie PUT, aby zaktualizować ogłoszenie o podanym ID
            await axios.put(`${ApiURL}/player-advertisements/${playerAdvertisementId}`, playerAdvertisement, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error updating player advertisement, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    async deletePlayerAdvertisement(playerAdvertisementId: number): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wykonujemy zapytanie DELETE, aby usunąć ogłoszenie o podanym ID
            await axios.delete(`${ApiURL}/player-advertisements/${playerAdvertisementId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error deleting player advertisement, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    async exportPlayerAdvertisementsToCsv(): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wykonujemy zapytanie GET, aby pobrać dane w formie CSV
            const response = await axios.get(`${ApiURL}/player-advertisements/export`, {
                headers: {
                    'Authorization': authorizationHeader
                },
                responseType: 'blob'        // Oczekujemy danych w postaci pliku blob (np. CSV)
            });

            // Tworzymy link do pobrania pliku CSV
            const url = window.URL.createObjectURL(new Blob([response.data], { type: 'text/csv' }));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', 'player-advertisements.csv');

            document.body.appendChild(link);        // Dodajemy link do strony
            link.click();           // Symulujemy kliknięcie w link, aby pobrać plik
            link.remove();      // Usuwamy link po pobraniu
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error("Error exporting player advertisements to CSV, details:", error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    }
};

// Eksportujemy serwis, aby można go było wykorzystać w innych częściach aplikacji
export default PlayerAdvertisementService;