import type { UserDTO } from '../dtos/UserDTO';
import type { PlayerAdvertisement } from './PlayerAdvertisement';

export interface FavoritePlayerAdvertisement {
    id: number;
    playerAdvertisementId: number;
    playerAdvertisement: PlayerAdvertisement;
    userId: string;
    user: UserDTO;
}