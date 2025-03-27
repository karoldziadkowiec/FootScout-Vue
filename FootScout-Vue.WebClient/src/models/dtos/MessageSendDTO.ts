// Określa wiadomość wysyłaną w czacie, przechowując identyfikatory czatu, nadawcy, odbiorcy i treść.

export interface MessageSendDTO {
    chatId: number;
    senderId: string;
    receiverId: string;
    content: string;
}