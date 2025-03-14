import type { UserDTO } from '../dtos/UserDTO';

export interface Chat {
    id: number;
    user1Id: string;
    user1: UserDTO;
    user2Id: string;
    user2: UserDTO;
}