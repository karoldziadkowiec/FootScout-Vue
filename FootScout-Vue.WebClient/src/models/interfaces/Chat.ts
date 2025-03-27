import type { UserDTO } from '../dtos/UserDTO';

// Reprezentuje czat między dwoma użytkownikami, zawierając ich identyfikatory i dane DTO.

export interface Chat {
    id: number;
    user1Id: string;
    user1: UserDTO;
    user2Id: string;
    user2: UserDTO;
}