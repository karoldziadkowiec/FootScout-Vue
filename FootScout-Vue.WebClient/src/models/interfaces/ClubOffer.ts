import type { PlayerAdvertisement } from './PlayerAdvertisement';
import type { OfferStatus } from './OfferStatus';
import type { PlayerPosition } from './PlayerPosition';
import type { UserDTO } from '../dtos/UserDTO';

// Przedstawia ofertę klubu dla zawodnika, zawierając informacje o ogłoszeniu, statusie oferty, pozycji, 
// wynagrodzeniu i nadawcy oferty.

export interface ClubOffer {
    id: number;
    playerAdvertisementId: number;
    playerAdvertisement: PlayerAdvertisement;
    offerStatusId: number;
    offerStatus: OfferStatus;
    playerPositionId: number;
    playerPosition: PlayerPosition;
    clubName: string;
    league: string;
    region: string;
    salary: number;
    additionalInformation: string;
    creationDate: string;
    clubMemberId: string;
    clubMember: UserDTO;
}