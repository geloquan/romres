let addChildMode = false;
function successDeleteHosts() {
    console.log("successDeleteHosts()");
    $('#confirmationModal').remove();

    var modalHtml = `
        <div class="modal fade" id="confirmationModal" tabindex="-1" aria-labelledby="confirmationModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="confirmationModalLabel">Success!</h5>
                    </div>
                    <div class="modal-body">
                        Slot has been added to database.
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" id="confirmButton">Close</button>
                    </div>
                </div>
            </div>
        </div>
    `;

    $('body').append(modalHtml);

    $('#confirmationModal').modal('show');

    $('#confirmationModal').find('[data-dismiss="modal"]').on('click', function() {
        $('#confirmationModal').modal('hide');
    });

    $('#confirmButton').on('click', function() {
        setTimeout(function() {
            $('#confirmationModal').modal('hide');
            window.location.reload();
          }, 250);
    });
}

function sendAddChild(newChild) {
    console.log('new child: ', newChild);
    console.log(JSON.stringify(newChild, null, 2));
    $.ajax({
        url: '/host/add/child',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(newChild),
        success: function(result) {
            console.log('Successfully saved new slot: ', result);
            global_slot_object = result;
            processSlot(newChild.slot_id);
            displayTable(1);
        },
        error: function() {
            alert('An error occurred while loading the content.');
        }
    });
}
function confirmAddChild(invitationCode, slotName, radio, original_anchor_container) {
    console.log("confirmAddChild()");
    if (!invitationCode.value || !slotName.value) {
        alert('Please fill in all the required fields.');
        return false;
    }
    var newChild = {
        slot_id: parseInt(document.getElementById('slot-parent-id').innerText),
        user_id: userId,
        child_slot_invitation_code: invitationCode.value,
        child_slot_name: slotName.value,
        child_slot_is_reservable: isReservableCheckbox.checked ? true : false
    }
    console.log('new child: ', newChild);
    
    $('#confirmationModal').remove();
    
    var modalHtml = `
        <div class="modal fade" id="confirmationModal" tabindex="-1" aria-labelledby="confirmationModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="confirmationModalLabel">Confirm Add Slot</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        Are you sure you want to add new slot named <span id="confirmSlotName"></span>, invitation code <span id="confirmInvitationCode"></span>, and Is Reservable to <span id="confirmIsReservable"></span>?
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
    
    document.getElementById('confirmSlotName').textContent = slotName.value;
    document.getElementById('confirmInvitationCode').textContent = invitationCode.value;
    document.getElementById('confirmIsReservable').textContent = isReservableCheckbox.checked ? 'Yes' : 'No';
    
    $('#confirmationModal').modal('show');
    
    $('#confirmationModal').on('hidden.bs.modal', function () {
        $(this).remove(); 
    });
    
    $('#confirmationModal').find('[data-dismiss="modal"]').on('click', function() {
        $('#confirmationModal').modal('hide');
    });
    
    $('#confirmButton').on('click', function() {
        sendAddChild(newChild);
        $('#confirmationModal').modal('hide');
    });
}

function toogleAddChildMode(enable) {
    console.log("toogleAddChildMode()");
    
    var original = $('#child-anchor-container').html();
    console.log('true');
    const table = document.getElementById('slot-children-table');
    var newRow = document.getElementById('temporary-child-row');
    if (!newRow) {
        newRow = document.createElement('tr');
        newRow.id = 'temporary-row';
        
        const slotNameCell = document.createElement('td');
        const slotNameInput = document.createElement('input');
        slotNameInput.type = 'text';
        slotNameInput.placeholder = 'Enter Slot Name';
        slotNameInput.classList.add('form-control');
        slotNameCell.appendChild(slotNameInput);
        
        const isReservableCell = document.createElement('td');
        
        const checkboxWrapper = document.createElement('div');
        checkboxWrapper.classList.add('form-check');
        
        const isReservableCheckbox = document.createElement('input');
        isReservableCheckbox.type = 'checkbox';
        isReservableCheckbox.id = 'isReservableCheckbox';
        isReservableCheckbox.name = 'isReservable';
        isReservableCheckbox.value = 1; 
        isReservableCheckbox.classList.add('form-check-input');
        
        const checkboxLabel = document.createElement('label');
        checkboxLabel.setAttribute('for', 'isReservableCheckbox');
        checkboxLabel.textContent = 'Reservable'; // Set the label text as needed
        checkboxLabel.classList.add('form-check-label');
        
        checkboxWrapper.appendChild(isReservableCheckbox);
        checkboxWrapper.appendChild(checkboxLabel);
        
        isReservableCell.appendChild(checkboxWrapper);
        
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
        };
        invitationCodeCell.appendChild(document.createElement('br'));
        invitationCodeCell.appendChild(generateCodeAnchor);
        
        newRow.appendChild(slotNameCell);
        newRow.appendChild(isReservableCell);
        newRow.appendChild(invitationCodeCell);
        newRow.appendChild(document.createElement('td'));
        newRow.appendChild(document.createElement('td'));
        newRow.appendChild(document.createElement('td'));
        
        table.querySelector('tbody').appendChild(newRow);
        
        const saveAnchor = $('<a href="#" id="save-selected-rows">Save New Child</a>');
        saveAnchor.click(function() {
            console.log("Save New Child.click()");
            if (!invitationCodeInput.value || !slotNameInput.value) {
                console.log("may be empty: {invitationCodeInput, slotNameInput}");
                return;
            } 
            confirmAddChild(invitationCodeInput, slotNameInput, isReservableCheckbox, original);
        });
    
        const backAnchor = $('<a href="#" id="back-to-original">Back</a>');
        backAnchor.click(function() {
            document.getElementById('temporary-row').remove();
        });
        
        $('#child-anchor-container').empty().append(saveAnchor).append(" | ").append(backAnchor);
    }
}