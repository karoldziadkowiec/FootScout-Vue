import type { Achievements } from '../dtos/AchievementsDTO';
import type { UserDTO } from '../dtos/UserDTO';
import type { PlayerPosition } from './PlayerPosition';

// Opisuje historię klubową zawodnika, zawierając informacje o klubie, lidze, pozycji na boisku i osiągnięciach.

export interface ClubHistoryModel {
    id: number;
    playerPositionId: number;
    playerPosition: PlayerPosition;
    clubName: string;
    league: string;
    region: string;
    startDate: string;
    endDate: string;
    achievementsId: number;
    achievements: Achievements;
    playerId: string;
    player: UserDTO;
}