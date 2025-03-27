import type { UserDTO } from '../dtos/UserDTO';

// Reprezentuje zgłoszenie problemu przez użytkownika, zawierając tytuł, opis, datę utworzenia 
// i status rozwiązania.

export interface Problem {
    id: number;
    title: string;
    description: string;
    creationDate: string;
    isSolved: boolean;
    requesterId: string;
    requester: UserDTO;
}