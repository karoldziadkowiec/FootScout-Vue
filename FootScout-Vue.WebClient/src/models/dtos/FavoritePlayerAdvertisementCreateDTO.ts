// Definiuje dane do oznaczenia ogłoszenia zawodnika jako ulubione, zawierając jego identyfikator 
// i użytkownika.

export interface FavoritePlayerAdvertisementCreateDTO {
    playerAdvertisementId: number;
    userId: string;
}