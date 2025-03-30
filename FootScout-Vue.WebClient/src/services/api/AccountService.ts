import axios from 'axios';    // Import biblioteki do wykonywania żądań HTTP (np. pobieranie danych z serwera)
import Cookies from 'js-cookie';    // Import biblioteki do zarządzania plikami cookie, np. przechowywanie tokenów sesji
import { ApiURL } from '../../config/ApiURL';   // stała zawierająca adres URL API, który jest używany do komunikacji z backendem
import type { LoginDTO } from '../../models/dtos/LoginDTO';   // typ definiujący strukturę danych wymaganych do logowania użytkownika
import type { RegisterDTO } from '../../models/dtos/RegisterDTO';   // typ definiujący strukturę danych potrzebnych do rejestracji użytkownika
import { jwtDecode } from 'jwt-decode'    // funkcja do dekodowania tokenów JWT, aby uzyskać informacje zawarte w tokenie, takie jak dane użytkownika
import { Role } from '../../models/enums/Role';  // Enum definiujący różne role użytkowników w systemie - admin, użytkownik

// AccountService.ts - Serwis zarządzający autoryzacją użytkowników, tokenami JWT (JSON Web Token) oraz rolami w aplikacji.

// Używa axios do komunikacji z backendem (API) oraz Cookies do przechowywania tokenów autoryzacyjnych.

export const AccountService = {
  // Rejestracja użytkownika - wysyła dane rejestracyjne do API i zwraca odpowiedź.
  async registerUser(registerDTO: RegisterDTO) {
    try {
      const response = await axios.post(`${ApiURL}/account/register`, registerDTO);
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Registration failed, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Logowanie użytkownika - wysyła dane logowania do API, zapisuje token w Cookies i zwraca go.
  async login(loginDTO: LoginDTO) {
    try {
      const response = await axios.post(`${ApiURL}/account/login`, loginDTO);
      const token = response.data;

      if (token) {
        Cookies.set('AuthToken', token, { path: '/' });
        return token;
      }
      else {
        console.error('Token not found in response');
      }
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Login failed, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobiera token autoryzacyjny z ciasteczek.
  async getToken() {
    return Cookies.get('AuthToken') || null;
  },

  // Odczytuje czas wygaśnięcia tokena z dekodowanego JWT.
  async getTokenExpirationTime() {
    const token = await AccountService.getToken();
    if (token) {
      try {
        const decodedToken = jwtDecode<any>(token);
        return decodedToken['exp'];   // Czas wygaśnięcia tokena w sekundach od 1970-01-01.
      }
      catch (error) {
        console.error('Failed to get token expiration time:', error);
      }
    }
    return null;
  },

  // Sprawdza, czy token jest nadal ważny, jeśli wygasł - usuwa go.
  async isTokenAvailable(): Promise<boolean> {
    const expirationDate = await AccountService.getTokenExpirationTime();
    if (expirationDate) {
      try {
        const expirationTime = expirationDate * 1000;   // Konwersja na milisekundy.
        const currentTime = Date.now();

        if (currentTime < expirationTime) {
          return true;        // Token jest nadal ważny.
        } 
        else {
          Cookies.remove('AuthToken');      // Usunięcie tokena, jeśli wygasł.
          return false;
        }
      }
      catch (error) {
        console.error('Failed to check if token is avaiable:', error);
      }
    }
    return false;
  },

  // Pobiera rolę użytkownika z dekodowanego tokena JWT.
  async getRole() {
    const token = await AccountService.getToken();
    if (token) {
      try {
        const decodedToken = jwtDecode<any>(token);
        return decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      }
      catch (error) {
        console.error('Failed to decode token:', error);
      }
    }
    return null;
  },

  // Pobiera listę dostępnych ról użytkowników z API.
  async getRoles(): Promise<string[]> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      const response = await axios.get<string[]>(`${ApiURL}/account/roles`, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
      return response.data;
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error fetching roles, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Pobiera identyfikator użytkownika z tokena JWT.
  async getId() {
    const token = await AccountService.getToken();
    if (token) {
      try {
        const decodedToken = jwtDecode<any>(token);
        return decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
      }
      catch (error) {
        console.error('Failed to decode token:', error);
      }
    }
    return null;
  },

  // Generuje nagłówek autoryzacyjny dla żądań HTTP.
  async getAuthorizationHeader(): Promise<string> {
    const tokenType = 'Bearer';
    const tokenJWT = await AccountService.getToken();
    if (tokenJWT)
      return `${tokenType} ${tokenJWT}`;

    else
      throw new Error('Token is not available');
  },

  // Sprawdza, czy użytkownik jest uwierzytelniony (czy istnieje token w ciasteczkach).
  async isAuthenticated() {
    const token = await AccountService.getToken();
    return !!token;
  },

  // Sprawdza, czy użytkownik ma rolę administratora.
  async isRoleAdmin(): Promise<boolean> {
    const role = await AccountService.getRole();
    if (role === Role.Admin)
      return true;
    else
      return false;
  },

  // Sprawdza, czy użytkownik ma rolę standardowego użytkownika.
  async isRoleUser(): Promise<boolean> {
    const role = await AccountService.getRole();
    if (role === Role.User)
      return true;
    else
      return false;
  },

  // Wylogowanie użytkownika - usuwa token autoryzacyjny z ciasteczek.
  async logout() {
    Cookies.remove('AuthToken', { path: '/' });
  },

  // Nadanie roli administratora użytkownikowi o podanym ID.
  async makeAnAdmin(userId: string): Promise<void> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      await axios.post(`${ApiURL}/account/roles/make-admin/${userId}`, userId, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error making an admin, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  },

  // Nadanie roli standardowego użytkownika użytkownikowi o podanym ID.
  async makeAnUser(userId: string): Promise<void> {
    try {
      const authorizationHeader = await AccountService.getAuthorizationHeader();
      await axios.post(`${ApiURL}/account/roles/make-user/${userId}`, userId, {
        headers: {
          'Authorization': authorizationHeader
        }
      });
    }
    catch (error) {
      if (axios.isAxiosError(error)) {
        console.error('Error making an user, details:', error.response?.data || error.message);
      }
      else {
        console.error('Unexpected error:', error);
      }
      throw error;
    }
  }
};