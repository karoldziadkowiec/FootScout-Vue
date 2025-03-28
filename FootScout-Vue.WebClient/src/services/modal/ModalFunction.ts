// ModalFunction.ts - Funkcja odpowiedzialna za zarządzanie modalami na stronie

// Importowanie klasy Modal z biblioteki Bootstrap, która umożliwia zarządzanie modalami
import { Modal } from 'bootstrap';

// Funkcja zamykająca modal o podanym identyfikatorze
export const closeModal = (modalId: string) => {
  // Pobierz element modalu na podstawie jego ID
  const modalElement = document.getElementById(modalId);
  
  // Sprawdzamy, czy element z danym ID istnieje
  if (modalElement) {
    // Pobiera istniejącą instancję modalu (jeśli już jest aktywna),
    // lub tworzy nową, jeśli jeszcze nie istnieje
    const modalInstance = Modal.getInstance(modalElement) || new Modal(modalElement);
    // Ukryj modal (czyli go zamykamy)
    modalInstance.hide();
  }
};