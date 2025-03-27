// Definiuje podstawowy adres API aplikacji, łącząc numer portu serwera z lokalnym hostem

const ApiPORT: number = 7236; // ustaw numer portu serwera
export const ApiURL: string = `https://localhost:${ApiPORT}/api`; // zwróć gotowy string hosta z numerem portu