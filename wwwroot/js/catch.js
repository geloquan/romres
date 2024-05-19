function showConfirmationModal(onConfirm, onCancel, confirmationModalLabelText, modalBodyText, cancelModalButtonText, confirmSaveButtonText) {
    $('#confirmationModal').modal('show'); // Show the modal

    const confirmSaveButton = document.getElementById('confirmSaveButton');
    const closeModalButton = document.getElementById('closeModalButton');
    const cancelModalButton = document.getElementById('cancelModalButton');
    const confirmationModalLabel = document.getElementById('confirmationModalLabel');
    const modalBody = document.getElementById('modal-body');

    // Remove any existing event listeners to avoid multiple bindings
    confirmSaveButton.onclick = null;
    closeModalButton.onclick = null;
    cancelModalButton.onclick = null;
    
    // Assign new event listeners
    confirmSaveButton.onclick = function() {
        $('#confirmationModal').modal('hide'); // Hide the modal
        if (typeof onConfirm === 'function') {
            onConfirm(); // Call the confirm callback function
        }
    };

    const cancelAction = function() {
        $('#confirmationModal').modal('hide'); // Hide the modal
        if (typeof onCancel === 'function') {
            onCancel(); // Call the cancel callback function if provided
        }
    };

    closeModalButton.onclick = cancelAction;
    cancelModalButton.onclick = cancelAction;
}
