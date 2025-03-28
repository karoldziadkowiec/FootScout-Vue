import axios from 'axios';
import { ApiURL } from '../../config/ApiURL';
import { AccountService } from './AccountService';
import type { PlayerPosition } from '../../models/interfaces/PlayerPosition';

// Serwis do zarządzania pozycjami piłkarskimi, wykorzystujący axios do komunikacji z API
// Każda metoda komunikuje się z API w celu pobrania, utworzenia lub sprawdzenia danych.

const PlayerPositionService = {
    // Metoda do pobierania wszystkich pozycji piłkarskich
    async getPlayerPositions(): Promise<PlayerPosition[]> {
        try {
            // Pobieranie nagłówka autoryzacji z AccountService
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wykonanie zapytania GET do API, aby pobrać listę pozycji
            const response = await axios.get<PlayerPosition[]>(`${ApiURL}/player-positions`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching player positions, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Metoda do pobierania liczby dostępnych pozycji piłkarskich
    async getPlayerPositionCount(): Promise<number> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wykonanie zapytania GET, aby pobrać liczbę pozycji
            const response = await axios.get<number>(`${ApiURL}/player-positions/count`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching player positions count, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Metoda do pobierania nazwy pozycji piłkarskiej na podstawie jej ID
    async getPlayerPositionName(positionId: number): Promise<string> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wykonanie zapytania GET, aby pobrać nazwę pozycji na podstawie jej ID
            const response = await axios.get<string>(`${ApiURL}/player-positions/${positionId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching selected player position, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Metoda sprawdzająca, czy pozycja piłkarska o danej nazwie istnieje
    async checkPlayerPositionExists(positionName: string): Promise<boolean> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wykonanie zapytania GET, aby sprawdzić istnienie pozycji
            const response = await axios.get<boolean>(`${ApiURL}/player-positions/check/name/${positionName}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;       // Zwrócenie wartości true/false w zależności od wyniku
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error checking existance of player position, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Metoda do tworzenia nowej pozycji piłkarskiej
    async createPlayerPosition(playerPosition: PlayerPosition): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wykonanie zapytania POST, aby stworzyć nową pozycję
            await axios.post(`${ApiURL}/player-positions`, playerPosition, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error creating new player position, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },
};

// Eksportowanie serwisu, aby można było go używać w innych częściach aplikacji
export default PlayerPositionService;