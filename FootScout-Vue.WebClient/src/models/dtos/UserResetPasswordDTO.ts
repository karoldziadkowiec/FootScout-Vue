// Struktura danych do resetowania hasła, zawierająca nowe hasło i jego potwierdzenie.

export interface UserResetPasswordDTO {
    passwordHash: string;
    confirmPasswordHash: string;
}