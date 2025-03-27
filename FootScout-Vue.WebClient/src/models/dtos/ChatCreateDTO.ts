// Określa dane wymagane do utworzenia czatu, zawierając identyfikatory dwóch użytkowników.

export interface ChatCreateDTO {
    user1Id: string;
    user2Id: string;
}