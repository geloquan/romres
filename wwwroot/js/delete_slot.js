function sendDeleteSlot(to_delete, parent_slot_id, slotName) {
    console.log('sendDeleteSlot()');
    $.ajax({
        url: '/slot/delete',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(to_delete),
        success: function(result) {
            //deleteRow('slot-children-table', to_delete.slot_id);
            console.log('before delete global: ', global_slot_object);
            deleteSlot(to_delete.slot_id, slotName);
            console.log('after delete global: ', global_slot_object);
            processSlot(parent_slot_id);
        },
        error: function() {
            alert('sendDeleteSlot() FAILED.');
        }
    });
}
function confirmDeleteSlot(slotName, invitationCode, slot_id, parent_slot_id) {
    console.log("confirmDeleteSlot()");
    
    var to_delete = {
        slot_id: slot_id,
        user_id: userId
    }
    console.log('to_delete: ', to_delete);
    
    $('#confirmationModal').remove();

    var modalHtml = `
        <div class="modal fade" id="confirmationModal" tabindex="-1" aria-labelledby="confirmationModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="confirmationModalLabel">Confirm Slot Deletion</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        Are you sure you want to delete slot named <span id="confirmSlotName"></span> — invitation code <span id="confirmInvitationCode"></span>?
                        <br>
                        <b>Note that its children will also be deleted.</b>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <button type="button" class="btn btn-primary" id="confirmButton">Confirm</button>
                    </div>
                </div>
            </div>
        </div>
    `;

    $('body').append(modalHtml);

    document.getElementById('confirmSlotName').textContent = slotName;
    document.getElementById('confirmInvitationCode').textContent = invitationCode;

    $('#confirmationModal').modal('show');

    $('#confirmationModal').on('hidden.bs.modal', function () {
        $(this).remove(); 
    });

    $('#confirmationModal').find('[data-dismiss="modal"]').on('click', function() {
        $('#confirmationModal').modal('hide');
    });

    $('#confirmButton').on('click', function() {
        sendDeleteSlot(to_delete, parent_slot_id, slotName);
        $('#confirmationModal').modal('hide');
    });
}