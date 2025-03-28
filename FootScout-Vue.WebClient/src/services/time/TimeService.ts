import { format } from 'date-fns';

// TimeService.ts - Serwis do manipulacji datami i obliczeń związanych z czasem

// Serwis do konwertowania/formatowania czasu i dat
export const TimeService = {
  getCurrentDateTime(): string {
    const now = new Date();
    const day = String(now.getDate()).padStart(2, '0');         // Dzień, zapewnia 2 cyfry
    const month = String(now.getMonth() + 1).padStart(2, '0');  // Miesiąc, zapewnia 2 cyfry
    const year = now.getFullYear();                             // Rok
    const hours = String(now.getHours()).padStart(2, '0');      // Godzina, zapewnia 2 cyfry
    const minutes = String(now.getMinutes()).padStart(2, '0');  // Minuty, zapewnia 2 cyfry
    const seconds = String(now.getSeconds()).padStart(2, '0');  // Sekundy, zapewnia 2 cyfry

    // Zwraca datę w wymaganym formacie
    return `${day}.${month}.${year} ${hours}:${minutes}:${seconds}`;
  },

  // Funkcja formatująca datę w formacie "dd.MM.yyyy HH:mm", zgodnie z europejskim formatem
  formatDateToEURWithHour(dateString: string): string {
    if (!dateString)
      return 'Invalid date';    // Jeśli nie podano daty, zwraca błąd

    const date = new Date(dateString);    // Tworzy obiekt Date z podanego ciągu znaków
    if (isNaN(date.getTime()))
      return '-';               // Sprawdza, czy data jest poprawna, jeśli nie, zwraca '-'

    return format(date, 'dd.MM.yyyy HH:mm');    // Używa zewnętrznej funkcji do sformatowania daty
  },

  // Funkcja formatująca datę w formacie "dd.MM.yyyy"
  formatDateToEUR(dateString: string): string {
    const date = new Date(dateString);      // Tworzy obiekt Date z podanego ciągu znaków
    const day = String(date.getDate()).padStart(2, '0');      // Dzień, zapewnia 2 cyfry
    const month = String(date.getMonth() + 1).padStart(2, '0');     // Miesiąc, zapewnia 2 cyfry
    const year = date.getFullYear();    // Rok

    // Zwraca datę w formacie "dd.MM.yyyy"
    return `${day}.${month}.${year}`;
  },

  // Funkcja formatująca datę w formacie "yyyy-MM-dd" do użycia w formularzach
  formatDateToForm(dateString: string): string {
    const date = new Date(dateString);    // Tworzy obiekt Date z podanego ciągu znaków
    const day = String(date.getDate()).padStart(2, '0');      // Dzień, zapewnia 2 cyfry
    const month = String(date.getMonth() + 1).padStart(2, '0');   // Miesiąc, zapewnia 2 cyfry
    const year = date.getFullYear();      // Rok  

    // Zwraca datę w formacie "yyyy-MM-dd"
    return `${year}-${month}-${day}`;     
  },

  // Funkcja obliczająca liczbę dni, które minęły od podanej daty do dzisiaj
  calculateSkippedDays(endDate: string): number {
    const currentDate = new Date();     // Bieżąca data
    const end = new Date(endDate);      // Data końcowa
    const timeDiff = currentDate.getTime() - end.getTime();   // Różnica czasowa w milisekundach
    const days = timeDiff / (1000 * 3600 * 24);     // Konwertuje różnicę na dni
    return Math.floor(days);      // Zwraca pełne dni, zaokrąglając w dół
  },

  // Funkcja obliczająca liczbę dni pozostałych do podanej daty
  calculateDaysLeft(endDate: string): number {
    const currentDate = new Date();     // Bieżąca data
    const end = new Date(endDate);      // Data końcowa
    const timeDiff = end.getTime() - currentDate.getTime(); // Różnica czasowa w milisekundach
    const daysLeft = timeDiff / (1000 * 3600 * 24);   // Konwertuje różnicę na dni
    return Math.ceil(daysLeft);   // Zwraca liczbę dni zaokrągloną w górę
  },

  // Funkcja obliczająca liczbę dni pozostałych lub minionych do podanej daty i zwracająca odpowiednią informację
  calculateDaysLeftPassed(endDate: string): string {
    const currentDate = new Date();   // Bieżąca data
    const end = new Date(endDate);    // Data końcowa

    if (currentDate <= end) {
      const timeDiff = end.getTime() - currentDate.getTime(); // Różnica czasowa, jeśli data jest w przyszłości
      const daysLeft = timeDiff / (1000 * 3600 * 24);         // Konwertuje różnicę na dni
      return `${Math.ceil(daysLeft)} days left`;              // Zwraca liczbę dni pozostałych
    }
    else {
      const timeDiff = currentDate.getTime() - end.getTime();  // Różnica czasowa, jeśli data jest w przeszłości
      const days = timeDiff / (1000 * 3600 * 24);             // Konwertuje różnicę na dni
      return `${Math.floor(days)} days passed`;               // Zwraca liczbę dni minionych
    }
  },

  // Funkcja generująca tablicę dat w formacie "yyyy-MM-dd" pomiędzy dwiema datami
  generateDateRange(startDate: string, endDate: string): string[] {
    const start = new Date(startDate);    // Data początkowa
    const end = new Date(endDate);        // Data końcowa
    const dateArray: string[] = [];       // Tablica na daty

    while (start <= end) {        // Dopóki data początkowa jest mniejsza lub równa dacie końcowej
      dateArray.push(start.toISOString().split('T')[0]);      // Dodaje datę do tablicy w formacie "yyyy-MM-dd"
      start.setDate(start.getDate() + 1);           // Przechodzi do następnego dnia
    }

    // Zwraca tablicę dat
    return dateArray;
  },

  // Funkcja zwracająca datę w formacie "yyyy-MM"
  formatDateToMonth(date: Date): string {
    return date.toISOString().substring(0, 7);    // Zwraca datę w formacie "yyyy-MM"
  },
};