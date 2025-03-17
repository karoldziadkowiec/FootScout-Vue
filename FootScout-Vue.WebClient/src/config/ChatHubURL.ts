const ApiPORT: number = 7236; // ustaw numer portu serwera
const HubName: string = 'chathub'; // ustaw nazwę huba w WebSocket od SignalR

export const ChatHubURL: string = `https://localhost:${ApiPORT}/${HubName}`; // zwróć gotowy string hosta z nazwą huba oraz numerem portu