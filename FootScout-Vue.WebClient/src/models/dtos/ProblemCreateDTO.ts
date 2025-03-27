// Reprezentuje zgłoszenie problemu przez użytkownika, zawierając tytuł, opis 
// i identyfikator zgłaszającego.

export interface ProblemCreateDTO {
    title: string;
    description: string;
    requesterId: string;
}