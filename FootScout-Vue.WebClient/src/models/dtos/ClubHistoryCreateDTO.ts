import type { Achievements } from './AchievementsDTO';

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