import axios from 'axios';
import { ApiURL } from '../../config/ApiURL';
import { AccountService } from './AccountService';
import type { PlayerFoot } from '../../models/interfaces/PlayerFoot';

// Serwis do zarządzania preferowanymi nogami piłkarzy, wykorzystujący axios do komunikacji z API

const PlayerFootService = {
    // Funkcja asynchroniczna do pobierania wszystkich preferencji nóg piłkarzy
    async getPlayerFeet(): Promise<PlayerFoot[]> {
        try {
            // Pobiera nagłówek autoryzacyjny (token) z serwisu AccountService
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysyła zapytanie GET do API, aby pobrać dane o nogach piłkarzy
            const response = await axios.get<PlayerFoot[]>(`${ApiURL}/player-feet`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                // Loguje szczegóły błędu, jeśli jest to błąd związany z axios
                console.error('Error fetching player feet, details:', error.response?.data || error.message);
            }
            else {
                // Loguje błędy, które nie są związane z axios (np. błąd sieci)
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja asynchroniczna do pobierania nazwy nogi piłkarza na podstawie ID
    async getPlayerFootName(footId: number): Promise<string> {
        try {
            // Pobiera nagłówek autoryzacyjny (token) z serwisu AccountService
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysyła zapytanie GET do API, aby pobrać nazwę nogi piłkarza na podstawie ID
            const response = await axios.get<string>(`${ApiURL}/player-feet/${footId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                // Loguje szczegóły błędu, jeśli jest to błąd związany z axios
                console.error('Error fetching selected player foot, details:', error.response?.data || error.message);
            }
            else {
                // Loguje błędy, które nie są związane z axios (np. błąd sieci)
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    }
};

// Eksportuje serwis, aby mógł być używany w innych częściach aplikacji
export default PlayerFootService;