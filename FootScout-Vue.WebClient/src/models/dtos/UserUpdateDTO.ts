// Definiuje dane do aktualizacji profilu użytkownika, obejmując imię, nazwisko, telefon i lokalizację.

export interface UserUpdateDTO {
    firstName: string;
    lastName: string;
    phoneNumber: string;
    location: string;
}