function confirmNewHost(slotName, invitationCode, hostNameTag, newSlot) {

    $('#confirmationModal').remove();

    var modalHtml = `
        <div class="modal fade" id="confirmationModal" tabindex="-1" aria-labelledby="confirmationModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="confirmationModalLabel">Confirm New Slot</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        Are you sure you want to create a new slot named <span id="confirmSlotName"></span>, invitation code <span id="confirmInvitationCode"></span>, and host tag-name <span id="confirmHostNameTag"></span>?
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
    document.getElementById('confirmHostNameTag').textContent = hostNameTag;

    $('#confirmationModal').modal('show');

    $('#confirmationModal').on('hidden.bs.modal', function () {
        $(this).remove(); // Remove the modal from the DOM after it is closed
    });

    $('#confirmationModal').find('[data-dismiss="modal"]').on('click', function() {
        $('#confirmationModal').modal('hide');
    });

    $('#confirmButton').on('click', function() {
        performAjaxSave(newSlot);
        $('#confirmationModal').modal('hide');
    });
}

function newRow() {
    const table = document.getElementById('hosted-slot-table');
    const newRow = document.createElement('tr');
    newRow.id = 'temporary-row';

    const invitationCodeCell = document.createElement('td');
    const invitationCodeInput = document.createElement('input');
    invitationCodeInput.type = 'text';
    invitationCodeInput.placeholder = 'Enter Invitation Code';
    invitationCodeInput.classList.add('form-control');
    invitationCodeCell.appendChild(invitationCodeInput);
    const generateCodeAnchor = document.createElement('a');
    generateCodeAnchor.href = '#';
    generateCodeAnchor.textContent = 'Generate Code';
    generateCodeAnchor.onclick = function(e) {
        e.preventDefault();
        invitationCodeInput.value = generateRandomCode();
        checkEnableButtons(invitationCodeInput, slotNameInput, hostNameTagInput, entryButton, unhostButton);
    };
    invitationCodeCell.appendChild(document.createElement('br'));
    invitationCodeCell.appendChild(generateCodeAnchor);
    
    const entryCell = document.createElement('td');
    const entryButton = document.createElement('button');
    entryButton.textContent = 'Enter Slot';
    entryButton.classList.add('btn', 'btn-primary');
    entryButton.disabled = true; // Initially disabled
    entryButton.onclick = function() {
    };
    entryCell.appendChild(entryButton);

    const slotNameCell = document.createElement('td');
    const slotNameInput = document.createElement('input');
    slotNameInput.type = 'text';
    slotNameInput.placeholder = 'Enter Slot Name';
    slotNameInput.classList.add('form-control');
    slotNameInput.oninput = function() {
        checkEnableButtons(invitationCodeInput, slotNameInput, hostNameTagInput, entryButton, unhostButton);
    };
    slotNameCell.appendChild(slotNameInput);

    
    const hostNameTagCell = document.createElement('td');
    const hostNameTagInput = document.createElement('input');
    hostNameTagInput.type = 'text';
    hostNameTagInput.placeholder = 'Enter Host Name-tag';
    hostNameTagInput.classList.add('form-control');
    hostNameTagInput.oninput = function() {
        checkEnableButtons(invitationCodeInput, slotNameInput, hostNameTagInput, entryButton, unhostButton);
    };
    hostNameTagCell.appendChild(hostNameTagInput);

    const unhostCell = document.createElement('td');
    const unhostButton = document.createElement('button');
    unhostButton.textContent = 'Unhost';
    unhostButton.classList.add('btn', 'btn-primary');
    unhostButton.disabled = true; // Initially disabled
    unhostCell.appendChild(unhostButton);

    newRow.appendChild(invitationCodeCell);
    newRow.appendChild(slotNameCell);
    newRow.appendChild(hostNameTagCell);
    newRow.appendChild(entryCell);
    newRow.appendChild(unhostCell);

    table.querySelector('tbody').appendChild(newRow);
}
function toogleNewHostMode(enable) {
    console.log("toogleNewHostMode()");
    const $buttons = $('#hosted-slot-table .btn');
    const $rows = $('#hosted-slot-table tbody tr');
    
    var original = $('#anchor-container').html();
    console.log("original: ", original);

    if (enable) {
        $buttons.prop('disabled', true);
        newRow();
        
        const saveAnchor = $('<a href="#" id="save-selected-rows">Save New Host</a>');
        saveAnchor.click(function() {
            if (!invitationCodeInput.value || !slotNameInput.value || !hostNameTagInput.value) {
                console.log("may be empty: {invitationCodeInput, slotNameInput, hostNameTagInput}");
                return;
            } 
            confirmNewHost(invitationCodeInput.value, slotNameInput.value, hostNameTagInput.value);
            console.log("Save New Host.click()");
        });
        
        const backAnchor = $('<a href="#" id="back-to-original">Back</a>');
        backAnchor.click(function() {
            toggleDeleteMode(false);
            $('#anchor-container').html(original);
            $('#delete-multiple-host').click(function() {
                toggleDeleteMode(true);
            });
            $('#new-host').click(function() {
                toogleNewHostMode(true);
            });
        });
        
        $('#anchor-container').empty().append(saveAnchor).append(" | ").append(backAnchor);
        $rows.addClass('pointer').on('click', selectRow);
    } else {
        $buttons.prop('disabled', false);
        $('#anchor-container').html(original);
        $rows.removeClass('pointer selected').off('click', selectRow);
        selectedRows.length = 0; 
    }
}