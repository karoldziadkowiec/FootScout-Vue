import type { Achievements } from './AchievementsDTO';

// Reprezentuje historię klubu zawodnika, zawierając informacje o pozycji, nazwie klubu, lidze, 
// regionie, datach oraz osiągnięciach.

export interface ClubHistoryCreateDTO {
    playerPositionId: number;
    clubName: string;
    league: string;
    region: string;
    startDate: string;
    endDate: string;
    achievements: Achievements;
    playerId: string;
}