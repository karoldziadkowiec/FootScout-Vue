import { Modal } from 'bootstrap';

// Funkcja zamykająca modal o podanym identyfikatorze
export const closeModal = (modalId: string) => {
  const modalElement = document.getElementById(modalId); // Pobierz element modalu na podstawie jego ID
  
  if (modalElement) {
    // Pobiera istniejącą instancję modalu lub tworzy nową, jeśli jeszcze nie istnieje
    const modalInstance = Modal.getInstance(modalElement) || new Modal(modalElement);
    // Ukryj modal
    modalInstance.hide();
  }
};