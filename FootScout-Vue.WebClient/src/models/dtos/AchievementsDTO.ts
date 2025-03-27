// Definiuje osiągnięcia gracza, takie jak liczba meczów, gole, asysty i dodatkowe osiągnięcia.

export interface Achievements {
    numberOfMatches: number;
    goals: number;
    assists: number;
    additionalAchievements: string;
}