// Definiuje dane wymagane do rejestracji użytkownika, w tym e-mail, hasło, imię, nazwisko, 
// numer telefonu i lokalizację.

export interface RegisterDTO {
    email: string;
    password: string;
    confirmPassword: string;
    firstName: string;
    lastName: string;
    phoneNumber: string;
    location: string;
}