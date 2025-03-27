import type { UserDTO } from '../dtos/UserDTO';
import type { PlayerAdvertisement } from './PlayerAdvertisement';

// Reprezentuje ulubione ogłoszenie piłkarskie użytkownika, łącząc je z danym użytkownikiem.

export interface FavoritePlayerAdvertisement {
    id: number;
    playerAdvertisementId: number;
    playerAdvertisement: PlayerAdvertisement;
    userId: string;
    user: UserDTO;
}