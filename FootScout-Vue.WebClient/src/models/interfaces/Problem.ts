import type { UserDTO } from '../dtos/UserDTO';

export interface Problem {
    id: number;
    title: string;
    description: string;
    creationDate: string;
    isSolved: boolean;
    requesterId: string;
    requester: UserDTO;
}