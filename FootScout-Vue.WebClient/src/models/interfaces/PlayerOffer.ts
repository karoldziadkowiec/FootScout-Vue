import type { ClubAdvertisement } from './ClubAdvertisement';
import type { OfferStatus } from './OfferStatus';
import type { PlayerPosition } from './PlayerPosition';
import type { PlayerFoot } from './PlayerFoot';
import type { UserDTO } from '../dtos/UserDTO';

export interface PlayerOffer {
    id: number;
    clubAdvertisementId: number;
    clubAdvertisement: ClubAdvertisement;
    offerStatusId: number;
    offerStatus: OfferStatus;
    playerPositionId: number;
    playerPosition: PlayerPosition;
    age: number;
    height: number;
    playerFootId: number;
    playerFoot: PlayerFoot;
    salary: number;
    additionalInformation: string;
    creationDate: string;
    playerId: string;
    player: UserDTO;
}