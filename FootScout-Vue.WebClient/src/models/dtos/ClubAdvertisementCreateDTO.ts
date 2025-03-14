import type { SalaryRangeCreateDTO } from "./SalaryRangeCreateDTO";

export interface ClubAdvertisementCreateDTO {
    playerPositionId: number;
    clubName: string;
    league: string;
    region: string;
    salaryRangeDTO: SalaryRangeCreateDTO;
    clubMemberId: string;
}