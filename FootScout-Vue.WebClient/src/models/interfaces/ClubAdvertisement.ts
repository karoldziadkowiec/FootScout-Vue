import type { PlayerPosition } from './PlayerPosition';
import type { SalaryRange } from "./SalaryRange";
import type { UserDTO } from '../dtos/UserDTO';

export interface ClubAdvertisement {
    id: number;
    playerPositionId: number;
    playerPosition: PlayerPosition;
    clubName: string;
    league: string;
    region: string;
    salaryRangeId: number;
    salaryRange: SalaryRange;
    creationDate: string;
    endDate: string;
    clubMemberId: string;
    clubMember: UserDTO;
}