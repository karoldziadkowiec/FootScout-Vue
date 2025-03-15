import { Modal } from 'bootstrap';

export const closeModal = (modalId: string) => {
  const modalElement = document.getElementById(modalId);
  if (modalElement) {
    const modalInstance = Modal.getInstance(modalElement) || new Modal(modalElement);
    modalInstance.hide();
  }
};