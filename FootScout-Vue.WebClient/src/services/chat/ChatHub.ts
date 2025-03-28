import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ChatHubURL } from '../../config/ChatHubURL';
import type { Message } from '../../models/interfaces/Message';
import type { MessageSendDTO } from '../../models/dtos/MessageSendDTO';

// Klasa do pracy z czatem w czasie rzeczywistym
class ChatHub {
    private connection: HubConnection | null = null; // Obiekt do przechowywania połączenia SignalR
    private chatId: number | null = null; // ID aktualnego czatu, na którym jest użytkownik
    
    // Callback wywoływany przy otrzymaniu wiadomości
    private onMessageReceived: (message: Message) => void;

    constructor(onMessageReceived: (message: Message) => void) {
        // Przypisanie funkcji callback, która będzie reagować na odebrane wiadomości
        this.onMessageReceived = onMessageReceived;
    }

    // Rozpoczyna połączenie SignalR dla danego czatu
    public async startConnection(chatId: number): Promise<void> {
        this.chatId = chatId;   // Zapisywanie ID czatu
        this.connection = new HubConnectionBuilder()
            .withUrl(ChatHubURL) // Ustawia URL dla SignalR huba
            .withAutomaticReconnect() // Włączenie automatycznego ponownego połączenia w przypadku problemów z siecią
            .build();       // Budowanie połączenia


        try {
            // Nawiązuje połączenie z hubem
            await this.connection.start();
            console.log("Joined the chat.");

            // Po udanym połączeniu informujemy serwer, że użytkownik chce dołączyć do czatu
            await this.connection.invoke("JoinChat", chatId);

            // Rejestracja funkcji odbierającej wiadomości i wywołującej callback przekazany w konstruktorze
            this.connection.on("ReceiveMessage", (message: Message) => {
                this.onMessageReceived(message);    // Wywołanie callbacka przy otrzymaniu wiadomości
            });
        } 
        catch (error) {
            console.error('Failed to connect to SignalR hub:', error);
            throw new Error('Failed to connect to chat.');
        }
    }

    // Wysyłanie wiadomości na serwer
    public async sendMessage(messageSendDTO: MessageSendDTO): Promise<void> {
        if (this.connection) {      // Sprawdzamy, czy połączenie zostało nawiązane
            try {
                const startTime = performance.now(); // Zapisujemy czas rozpoczęcia operacji wysyłania wiadomości
                
                await this.connection.invoke("SendMessage", messageSendDTO); // Wysyłamy wiadomość do serwera
                
                const endTime = performance.now(); // Zapisujemy, pobieramy czas zakończenia operacji
                const timeTaken = endTime - startTime;      // Obliczamy czas, jaki zajęło wysłanie wiadomości
                console.log(`Czas wysyłania wiadomości: ${Math.round(timeTaken)} ms`);      // Wyświetlamy czas wysyłania
            } 
            catch (error) {
                console.error('Failed to send message:', error);
                throw new Error('Failed to send message.');
            }
        }
    }

    // Opuszczanie czatu i zamykanie połączenia SignalR
    public async leaveChat(): Promise<void> {
        if (this.connection && this.chatId !== null) {      // Sprawdzamy, czy połączenie istnieje i mamy ID czatu
            try {
                await this.connection.invoke("LeaveChat", this.chatId); // Powiadamiamy serwer, że użytkownik opuszcza czat
                await this.connection.stop(); // Zatrzymujemy połączenie z hubem
            } 
            catch (error) {
                console.error('Failed to leave chat:', error);
                throw new Error('Failed to leave chat.');
            }
        }
    }
}

// Eksportowanie klasy, by mogła być używana w innych częściach aplikacji
export default ChatHub;