// Modeluje ofertę klubu dla zawodnika, uwzględniając dane takie jak liga, region, 
// wynagrodzenie i dodatkowe informacje.

export interface ClubOfferCreateDTO {
    playerAdvertisementId: number;
    playerPositionId: number;
    clubName: string;
    league: string;
    region: string;
    salary: number;
    additionalInformation: string;
    clubMemberId: string;
}