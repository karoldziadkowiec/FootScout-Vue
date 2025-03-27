// Reprezentuje pełne dane użytkownika, w tym identyfikator, e-mail, hasło, imię, nazwisko, 
// telefon, lokalizację i datę utworzenia konta.

export interface UserDTO {
    id: string;
    email: string;
    passwordHash: string;
    firstName: string;
    lastName: string;
    phoneNumber: string;
    location: string;
    creationDate: string;
}