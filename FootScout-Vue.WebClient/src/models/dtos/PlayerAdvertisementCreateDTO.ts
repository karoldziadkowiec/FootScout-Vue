import type { SalaryRangeCreateDTO } from "./SalaryRangeCreateDTO";

// Modeluje ogłoszenie zawodnika, uwzględniając dane o pozycji, lidze, wieku, wzroście, 
// preferowanej nodze i przedziale wynagrodzenia.

export interface PlayerAdvertisementCreateDTO {
    playerPositionId: number;
    league: string;
    region: string;
    age: number;
    height: number;
    playerFootId: number;
    salaryRangeDTO: SalaryRangeCreateDTO;
    playerId: string;
}