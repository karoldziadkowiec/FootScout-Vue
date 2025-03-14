import type { UserDTO } from '../dtos/UserDTO';
import type { ClubAdvertisement } from './ClubAdvertisement';

export interface FavoriteClubAdvertisement {
    id: number;
    clubAdvertisementId: number;
    clubAdvertisement: ClubAdvertisement;
    userId: string;
    user: UserDTO;
}