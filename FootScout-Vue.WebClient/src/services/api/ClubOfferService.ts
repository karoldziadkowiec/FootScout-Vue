import axios from 'axios';
import { ApiURL } from '../../config/ApiURL';
import { AccountService } from './AccountService';
import type { ClubOffer } from '../../models/interfaces/ClubOffer';
import type { ClubOfferCreateDTO } from '../../models/dtos/ClubOfferCreateDTO';

// Serwis do zarządzania ofertami klubów, wykorzystujący axios do komunikacji z API

const ClubOfferService = {
    // Pobiera ofertę klubu na podstawie ID oferty
    async getClubOffer(clubOfferId: number): Promise<ClubOffer> {
        try {
            // Uzyskanie nagłówka autoryzacji
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysyłanie zapytania GET do API
            const response = await axios.get<ClubOffer>(`${ApiURL}/club-offers/${clubOfferId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;       // Zwracanie danych odpowiedzi
        }
        catch (error) {
            // Obsługa błędów, zarówno specyficznych dla axios, jak i innych
            if (axios.isAxiosError(error)) {
                console.error('Error fetching club offer, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Pobiera wszystkie oferty klubów
    async getClubOffers(): Promise<ClubOffer[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<ClubOffer[]>(`${ApiURL}/club-offers`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;       // Zwracanie wszystkich ofert
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching all club offers, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Pobiera tylko aktywne oferty klubów
    async getActiveClubOffers(): Promise<ClubOffer[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<ClubOffer[]>(`${ApiURL}/club-offers/active`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching all active club offers, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Pobiera liczbę aktywnych ofert klubów
    async getActiveClubOfferCount(): Promise<number> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<number>(`${ApiURL}/club-offers/active/count`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching active club offer count, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Pobiera oferty klubów, które są nieaktywne
    async getInactiveClubOffers(): Promise<ClubOffer[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<ClubOffer[]>(`${ApiURL}/club-offers/inactive`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching all inactive club offers, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Tworzy nową ofertę klubu
    async createClubOffer(dto: ClubOfferCreateDTO): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysyłanie zapytania POST do API, by utworzyć nową ofertę
            await axios.post(`${ApiURL}/club-offers`, dto, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error creating new club offer, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Aktualizuje ofertę klubu
    async updateClubOffer(clubOfferId: number, clubOffer: ClubOffer): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysyłanie zapytania PUT do API w celu aktualizacji oferty
            await axios.put(`${ApiURL}/club-offers/${clubOfferId}`, clubOffer, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error updating club offer, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Usuwa ofertę klubu
    async deleteClubOffer(clubOfferId: number): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysyłanie zapytania DELETE do API, by usunąć ofertę
            await axios.delete(`${ApiURL}/club-offers/${clubOfferId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error deleting club offer, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Akceptuje ofertę klubu
    async acceptClubOffer(clubOfferId: number, clubOffer: ClubOffer): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysyłanie zapytania PUT do API w celu zaakceptowania oferty
            await axios.put(`${ApiURL}/club-offers/accept/${clubOfferId}`, clubOffer, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error accepting club offer, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Odrzuca ofertę klubu
    async rejectClubOffer(clubOfferId: number, clubOffer: ClubOffer): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysyłanie zapytania PUT do API w celu odrzucenia oferty
            await axios.put(`${ApiURL}/club-offers/reject/${clubOfferId}`, clubOffer, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error rejecting club offer, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Pobiera status oferty na podstawie ID ogłoszenia gracza i ID użytkownika
    async getClubOfferStatusId(playerAdvertisementId: number, userId: string): Promise<number> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<number>(`${ApiURL}/club-offers/status/${playerAdvertisementId}/${userId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error getting offer status id, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Eksportuje wszystkie oferty klubów do pliku CSV
    async exportClubOffersToCsv(): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();

            // Wysyłanie zapytania GET do API w celu pobrania danych w formacie CSV
            const response = await axios.get(`${ApiURL}/club-offers/export`, {
                headers: {
                    'Authorization': authorizationHeader
                },
                responseType: 'blob'    // Oczekiwanie pliku
            });

            // Tworzenie linku do pobrania pliku CSV
            const url = window.URL.createObjectURL(new Blob([response.data], { type: 'text/csv' }));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', 'club-offers.csv');

            document.body.appendChild(link);
            link.click();
            link.remove();
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error("Error exporting club offers to CSV, details:", error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    }
};

// Eksportuje serwis ofert klubu, który zawiera metody do zarządzania ofertami klubów,
// umożliwiając jego użycie w innych plikach aplikacji.
export default ClubOfferService;