// Definiuje osiągnięcia piłkarskie zawodnika, takie jak liczba meczów, gole, asysty i dodatkowe 
// wyróżnienia.

export interface Achievements {
    id: number;
    numberOfMatches: number;
    goals: number;
    assists: number;
    additionalAchievements: string;
}