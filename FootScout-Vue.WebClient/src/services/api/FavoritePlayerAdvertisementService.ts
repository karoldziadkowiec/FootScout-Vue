import axios from 'axios';
import { ApiURL } from '../../config/ApiURL';
import { AccountService } from './AccountService';
import type { FavoritePlayerAdvertisementCreateDTO } from '../../models/dtos/FavoritePlayerAdvertisementCreateDTO';

// Serwis do zarządzania polubionymi ogłoszeniami piłkarskimi, wykorzystujący axios do komunikacji z API

const FavoritePlayerAdvertisementService = {
    // Funkcja do dodawania ogłoszenia o ulubionym graczu do listy ulubionych
    async addToFavorites(dto: FavoritePlayerAdvertisementCreateDTO): Promise<void> {
        try {
            // Pobranie nagłówka autoryzacji z AccountService
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysyłanie żądania POST do API w celu dodania ogłoszenia do ulubionych
            await axios.post(`${ApiURL}/player-advertisements/favorites`, dto, {
                headers: {
                    'Authorization': authorizationHeader        // Dodanie nagłówka autoryzacji
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error adding player advertisement to favorites, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;        // Przekazanie błędu dalej
        }
    },

    // Funkcja do usuwania ogłoszenia o ulubionym graczu z listy ulubionych
    async deleteFromFavorites(favoritePlayerAdvertisementId: number): Promise<void> {
        try {
            // Pobranie nagłówka autoryzacji z AccountService
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysyłanie żądania DELETE do API w celu usunięcia ogłoszenia z ulubionych
            await axios.delete(`${ApiURL}/player-advertisements/favorites/${favoritePlayerAdvertisementId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error deleting player advertisement from favorites, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja do sprawdzania, czy dane ogłoszenie jest już na liście ulubionych danego użytkownika
    async checkPlayerAdvertisementIsFavorite(playerAdvertisementId: number, userId: string): Promise<number> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysyłanie żądania GET do API, aby sprawdzić, czy ogłoszenie jest ulubione
            const response = await axios.get<number>(`${ApiURL}/player-advertisements/favorites/check/${playerAdvertisementId}/${userId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            // Zwrócenie odpowiedzi z API (0 lub 1)
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error checking if player advertisement is favorite, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    }
};

// Eksportowanie serwisu, aby był dostępny w innych częściach aplikacji
export default FavoritePlayerAdvertisementService;