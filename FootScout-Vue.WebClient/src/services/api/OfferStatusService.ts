import axios from 'axios';
import { ApiURL } from '../../config/ApiURL';
import { AccountService } from './AccountService';
import type { OfferStatus } from '../../models/interfaces/OfferStatus';

// Serwis do zarządzania statusami ofert, wykorzystujący axios do komunikacji z API
const OfferStatusService = {
    // Funkcja pobierająca wszystkie statusy ofert
    async getOfferStatuses(): Promise<OfferStatus[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wykonywanie zapytania GET do API, aby pobrać wszystkie statusy ofert
            const response = await axios.get<OfferStatus[]>(`${ApiURL}/offer-statuses`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching offer statuses, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja pobierająca pojedynczy status oferty na podstawie jego identyfikatora
    async getOfferStatus(statusId: number): Promise<OfferStatus> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<OfferStatus>(`${ApiURL}/offer-statuses/${statusId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching offer status, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja pobierająca nazwę statusu oferty na podstawie identyfikatora statusu
    async getOfferStatusName(statusId: number): Promise<string> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<string>(`${ApiURL}/offer-statuses/name/${statusId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching selected offer name, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Funkcja pobierająca identyfikator statusu oferty na podstawie jego nazwy
    async getOfferStatusId(statusName: string): Promise<number> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<number>(`${ApiURL}/offer-statuses/id/${statusName}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching selected status id, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    }
};

// Eksportowanie serwisu, aby mógł być używany w innych częściach aplikacji
export default OfferStatusService;