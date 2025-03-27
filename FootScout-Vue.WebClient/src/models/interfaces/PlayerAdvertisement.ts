import type { PlayerPosition } from './PlayerPosition';
import type { PlayerFoot } from './PlayerFoot';
import type { SalaryRange } from "./SalaryRange";
import type { UserDTO } from '../dtos/UserDTO';

// Reprezentuje ogłoszenie piłkarza, zawierając jego pozycję, wiek, wzrost, preferowaną stopę, 
// zakres wynagrodzenia i daty ważności ogłoszenia.

export interface PlayerAdvertisement {
    id: number;
    playerPositionId: number;
    playerPosition: PlayerPosition;
    league: string;
    region: string;
    age: number;
    height: number;
    playerFootId: number;
    playerFoot: PlayerFoot;
    salaryRangeId: number;
    salaryRange: SalaryRange;
    creationDate: string;
    endDate: string;
    playerId: string;
    player: UserDTO;
}