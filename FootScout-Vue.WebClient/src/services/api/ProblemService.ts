import axios from 'axios';
import { ApiURL } from '../../config/ApiURL';
import { AccountService } from './AccountService';
import type { Problem } from '../../models/interfaces/Problem';
import type { ProblemCreateDTO } from '../../models/dtos/ProblemCreateDTO';

// Serwis do zarządzania zgłoszonymi problemami przez użytkowników, wykorzystujący axios do komunikacji z API

const ProblemService = {
    // Pobranie szczegółów pojedynczego problemu na podstawie jego ID
    async getProblem(problemId: number): Promise<Problem> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysłanie zapytania GET do API, aby uzyskać szczegóły problemu
            const response = await axios.get<Problem>(`${ApiURL}/problems/${problemId}`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching problem, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Pobranie wszystkich problemów
    async getAllProblems(): Promise<Problem[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<Problem[]>(`${ApiURL}/problems`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching all problems, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Pobranie wszystkich rozwiązanych problemów
    async getSolvedProblems(): Promise<Problem[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<Problem[]>(`${ApiURL}/problems/solved`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching all solved problems, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Pobranie liczby rozwiązanych problemów
    async getSolvedProblemCount(): Promise<number> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<number>(`${ApiURL}/problems/solved/count`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching solved problem count, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Pobranie wszystkich nierozwiązanych problemów
    async getUnsolvedProblems(): Promise<Problem[]> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<Problem[]>(`${ApiURL}/problems/unsolved`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching all unsolved problems, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Pobranie liczby nierozwiązanych problemów
    async getUnsolvedProblemCount(): Promise<number> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            const response = await axios.get<number>(`${ApiURL}/problems/unsolved/count`, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
            return response.data;
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error fetching unsolved problem count, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Utworzenie nowego problemu
    async createProblem(dto: ProblemCreateDTO): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysłanie zapytania POST do API, aby utworzyć nowy problem
            await axios.post(`${ApiURL}/problems`, dto, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error creating new problem, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Oznaczenie problemu jako rozwiązany
    async checkProblemSolved(problemId: number, problem: Problem): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysłanie zapytania PUT do API, aby zaktualizować status problemu
            await axios.put(`${ApiURL}/problems/${problemId}`, problem, {
                headers: {
                    'Authorization': authorizationHeader
                }
            });
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error('Error checking problem to solved, details:', error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    },

    // Eksportowanie listy problemów do pliku CSV
    async exportProblemsToCsv(): Promise<void> {
        try {
            const authorizationHeader = await AccountService.getAuthorizationHeader();
            // Wysłanie zapytania GET do API w celu pobrania danych w formacie CSV
            const response = await axios.get(`${ApiURL}/problems/export`, {
                headers: {
                    'Authorization': authorizationHeader
                },
                responseType: 'blob'        // Oczekiwanie na odpowiedź w formacie blob (plik)
            });

            const url = window.URL.createObjectURL(new Blob([response.data], { type: 'text/csv' }));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', 'problems.csv');

            document.body.appendChild(link);    // Kliknięcie w link, aby rozpocząć pobieranie
            link.click();
            link.remove();
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                console.error("Error exporting problems to CSV, details:", error.response?.data || error.message);
            }
            else {
                console.error('Unexpected error:', error);
            }
            throw error;
        }
    }
};

// Eksportowanie serwisu, aby można było go używać w innych częściach aplikacji
export default ProblemService;