import axios from 'axios';
import { ApiURL } from '../../config/ApiURL';
import { AccountService } from './AccountService';
import type { PlayerFoot } from '../../models/interfaces/PlayerFoot';

// Serwis do zarządzania preferowanymi nogami piłkarzy, wykorzystujący axios do komunikacji z API
const PlayerFootService = {
    async getPlayerFeet(): Promise<PlayerFoot[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<PlayerFoot[]>(`${ApiURL}/player-feet`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching player feet, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    async getPlayerFootName(footId: number): Promise<string> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<string>(`${ApiURL}/player-feet/${footId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching selected player foot, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    }
};

export default PlayerFootService;