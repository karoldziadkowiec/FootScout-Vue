import axios from 'axios';
import { ApiURL } from '../../config/ApiURL';
import { AccountService } from './AccountService';
import type { UserDTO } from '../../models/dtos/UserDTO';
import type { UserUpdateDTO } from '../../models/dtos/UserUpdateDTO';
import type { UserResetPasswordDTO } from '../../models/dtos/UserResetPasswordDTO';
import type { ClubHistoryModel } from '../../models/interfaces/ClubHistory';
import type { PlayerAdvertisement } from '../../models/interfaces/PlayerAdvertisement';
import type { FavoritePlayerAdvertisement } from '../../models/interfaces/FavoritePlayerAdvertisement';
import type { ClubOffer } from '../../models/interfaces/ClubOffer';
import type { Chat } from '../../models/interfaces/Chat';

// Serwis do zarządzania użytkownikami, wykorzystujący axios do komunikacji z backendowym API
const UserService = {
  // Pobieranie danych o użytkowniku na podstawie jego ID
  async getUser(userId: string): Promise<UserDTO> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      // Wysłanie zapytania GET do API z odpowiednim nagłówkiem autoryzacji
      const response = await axios.get<UserDTO>(`${ApiURL}/users/${userId}`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error fetching user, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobieranie wszystkich użytkowników
  async getUsers(): Promise<UserDTO[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<UserDTO[]>(`${ApiURL}/users`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error fetching users, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobieranie tylko użytkowników, którzy mają rolę "user"
  async getOnlyUsers(): Promise<UserDTO[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<UserDTO[]>(`${ApiURL}/users/role/user`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error fetching users from role user, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobieranie tylko użytkowników, którzy mają rolę "admin"
  async getOnlyAdmins(): Promise<UserDTO[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<UserDTO[]>(`${ApiURL}/users/role/admin`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error fetching users from role admin, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobieranie roli użytkownika na podstawie jego ID
  async getUserRole(userId: string): Promise<string> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<string>(`${ApiURL}/users/${userId}/role`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error fetching user role, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobieranie liczby wszystkich użytkowników
  async getUserCount(): Promise<number> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<number>(`${ApiURL}/users/count`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error fetching user count, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Aktualizacja danych użytkownika
  async updateUser(userId: string, dto: UserUpdateDTO): Promise<void> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      await axios.put(`${ApiURL}/users/${userId}`, dto, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error updating user, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Resetowanie hasła użytkownika
  async resetUserPassword(userId: string, dto: UserResetPasswordDTO): Promise<void> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      await axios.put(`${ApiURL}/users/reset-password/${userId}`, dto, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error reseting user\'s password, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Usuwanie użytkownika na podstawie jego ID
  async deleteUser(userId: string): Promise<void> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      await axios.delete(`${ApiURL}/users/${userId}`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error deleting user, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobieranie historii klubów użytkownika
  async getUserClubHistory(userId: string): Promise<ClubHistoryModel[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<ClubHistoryModel[]>(`${ApiURL}/users/${userId}/club-history`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's club histories, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Inne funkcje do pobierania reklam graczy, ofert klubów podobne w strukturze do powyższych funkcji.
  async getUserPlayerAdvertisements(userId: string): Promise<PlayerAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<PlayerAdvertisement[]>(`${ApiURL}/users/${userId}/player-advertisements`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's player advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobieranie aktywnych ogłoszeń zawodników użytkownika.
  async getUserActivePlayerAdvertisements(userId: string): Promise<PlayerAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<PlayerAdvertisement[]>(`${ApiURL}/users/${userId}/player-advertisements/active`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's active player advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobieranie nieaktywnych ogłoszeń zawodników użytkownika.
  async getUserInactivePlayerAdvertisements(userId: string): Promise<PlayerAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<PlayerAdvertisement[]>(`${ApiURL}/users/${userId}/player-advertisements/inactive`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's inactive player advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobieranie ulubionych ogłoszeń zawodników użytkownika.
  async getUserPlayerAdvertisementFavorites(userId: string): Promise<FavoritePlayerAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<FavoritePlayerAdvertisement[]>(`${ApiURL}/users/${userId}/player-advertisements/favorites`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's favorite player advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobieranie aktywnych ulubionych ogłoszeń zawodników użytkownika.
  async getUserActivePlayerAdvertisementFavorites(userId: string): Promise<FavoritePlayerAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<FavoritePlayerAdvertisement[]>(`${ApiURL}/users/${userId}/player-advertisements/favorites/active`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's active favorite player advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobieranie nieaktywnych ulubionych ogłoszeń zawodników użytkownika.
  async getUserInactivePlayerAdvertisementFavorites(userId: string): Promise<FavoritePlayerAdvertisement[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<FavoritePlayerAdvertisement[]>(`${ApiURL}/users/${userId}/player-advertisements/favorites/inactive`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's inactive favorite player advertisements, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobieranie otrzymanych ofert klubów dla użytkownika.
  async getReceivedClubOffers(userId: string): Promise<ClubOffer[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<ClubOffer[]>(`${ApiURL}/users/${userId}/club-offers/received`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's received club offers, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobieranie wysłanych ofert klubów przez użytkownika.
  async getSentClubOffers(userId: string): Promise<ClubOffer[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<ClubOffer[]>(`${ApiURL}/users/${userId}/club-offers/sent`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's sent club offers, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobieranie czatów przypisanych do użytkownika.
  async getUserChats(userId: string): Promise<Chat[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<Chat[]>(`${ApiURL}/users/${userId}/chats`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error fetching user's chats, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Eksportowanie użytkowników do pliku CSV
  async exportUsersToCsv(): Promise<void> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      // Wysłanie zapytania do API w celu pobrania danych użytkowników w formacie CSV
      const response = await axios.get(`${ApiURL}/users/export`, {
        headers: {
          'Authorization': authorizationHeader
        },
        responseType: 'blob'  // Ustawienie typu odpowiedzi na 'blob', aby pobrać plik
      });

      // Tworzenie linku do pobrania pliku
      const url = window.URL.createObjectURL(new Blob([response.data], { type: 'text/csv' }));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', 'users.csv');   // Ustalenie nazwy pliku do pobrania

      // Automatyczne kliknięcie w link, aby uruchomić pobieranie
      document.body.appendChild(link);
      link.click();
      link.remove();
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Error exporting users to CSV, details:", error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  }
};

// Eksportowanie serwisu, aby można było go używać w innych częściach aplikacji
export default UserService;