import type { UserDTO } from '../dtos/UserDTO';
import type { Chat } from './Chat';

// Definiuje wiadomość w czacie, przechowując treść, nadawcę, odbiorcę i znacznik czasu.

export interface Message {
    id: number; 
    chatId: number;
    chat: Chat;
    content: string;
    senderId: string;
    sender: UserDTO;
    receiverId: string;
    receiver: UserDTO;
    timestamp: string;
}