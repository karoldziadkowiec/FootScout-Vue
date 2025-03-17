import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ChatHubURL } from '../../config/ChatHubURL';
import type { Message } from '../../models/interfaces/Message';
import type { MessageSendDTO } from '../../models/dtos/MessageSendDTO';

// Klasa do pracy z czatem w czasie rzeczywistym
class ChatHub {
    private connection: HubConnection | null = null; // Obiekt połączenia SignalR
    private chatId: number | null = null; // ID aktualnego czatu
    
    // Callback wywoływany przy otrzymaniu wiadomości
    private onMessageReceived: (message: Message) => void;

    constructor(onMessageReceived: (message: Message) => void) {
        this.onMessageReceived = onMessageReceived;
    }

    // Rozpoczyna połączenie SignalR dla danego czatu
    public async startConnection(chatId: number): Promise<void> {
        this.chatId = chatId;
        this.connection = new HubConnectionBuilder()
            .withUrl(ChatHubURL) // Ustawia URL dla SignalR huba
            .withAutomaticReconnect() // Włącza automatyczne ponowne połączenie
            .build();

        try {
            // Nawiązuje połączenie z hubem
            await this.connection.start();
            console.log("Joined the chat.");

            // Informuje serwer, że użytkownik dołączył do czatu
            await this.connection.invoke("JoinChat", chatId);

            // Ustawia obsługę odbierania wiadomości
            this.connection.on("ReceiveMessage", (message: Message) => {
                this.onMessageReceived(message);
            });
        } 
        catch (error) {
            console.error('Failed to connect to SignalR hub:', error);
            throw new Error('Failed to connect to chat.');
        }
    }

    // Wysyłanie wiadomości na serwer
    public async sendMessage(messageSendDTO: MessageSendDTO): Promise<void> {
        if (this.connection) {
            try {
                const startTime = performance.now(); // Pobierz czas rozpoczęcia operacji
                
                await this.connection.invoke("SendMessage", messageSendDTO); // Wyślij wiadomość
                
                const endTime = performance.now(); // Pobierz czas zakończenia operacji
                const timeTaken = endTime - startTime;
                console.log(`Czas wysyłania wiadomości: ${Math.round(timeTaken)} ms`);
            } 
            catch (error) {
                console.error('Failed to send message:', error);
                throw new Error('Failed to send message.');
            }
        }
    }

    // Opuszczanie czatu i zamykanie połączenia SignalR
    public async leaveChat(): Promise<void> {
        if (this.connection && this.chatId !== null) {
            try {
                await this.connection.invoke("LeaveChat", this.chatId); // Informuj serwer o opuszczeniu czatu
                await this.connection.stop(); // Zatrzymaj połączenie
            } 
            catch (error) {
                console.error('Failed to leave chat:', error);
                throw new Error('Failed to leave chat.');
            }
        }
    }
}

export default ChatHub;