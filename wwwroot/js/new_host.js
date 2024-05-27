let newHostMode = false;

function sendNewHost(newHost) {
    console.log('sendNewHost()()()');
    $.ajax({
        url: '/host/newhost',
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(newHost),
        success: function(result) {
            console.log('Successfully saved new slot: ', result);
            var dummy_tree = createDummyRootTrees(newHost.invitationCode, newHost.slotName, newHost.hostNameTag, result.new_slot_id);
            console.log('dummy tree: ', dummy_tree);
            console.log('original tree struct: ', global_slot_object);
            global_slot_object.slotTrees.push(dummy_tree.slotTrees[0]);
            console.log('after push struct: ', global_slot_object);
            HostedSlots(dummy_tree);
        },
        error: function() {
            alert('An error occurred while loading the content.');
        }
    });
}

function confirmNewHost(invitationCode, slotName, hostNameTag, newSlot, original_anchor_container) {
    console.log("confirmNewHost()");
    if (!invitationCode.value || !slotName.value || !hostNameTag.value) {
        alert('Please fill in all the required fields.');
        return false;
    }
    invitationCode.disabled = true;
    slotName.disabled = true;
    hostNameTag.disabled = true;
    
    var newHost = {
        hostNameTag: hostNameTag.value,
        invitationCode: invitationCode.value,
        slotName: slotName.value,
        userId: userId
    };

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

    document.getElementById('confirmSlotName').textContent = slotName.value;
    document.getElementById('confirmInvitationCode').textContent = invitationCode.value;
    document.getElementById('confirmHostNameTag').textContent = hostNameTag.value;

    $('#confirmationModal').modal('show');

    $('#confirmationModal').on('hidden.bs.modal', function () {
        $(this).remove(); 
    });

    $('#confirmationModal').find('[data-dismiss="modal"]').on('click', function() {
        $('#confirmationModal').modal('hide');
    });

    $('#confirmButton').on('click', function() {
        sendNewHost(newHost);
        $('#confirmationModal').modal('hide');
        toogleNewHostMode(false);
        $('#anchor-container').html(original_anchor_container);
        $('#delete-multiple-host').click(function() {
            toggleDeleteMode(true);
        });
        $('#new-host').click(function() {
            toogleNewHostMode(true);
        });
    });
}

function newRow() {
}
function toogleNewHostMode(enable) {
    console.log("toogleNewHostMode()");
    const $buttons = $('#hosted-slot-table .btn');
    const $rows = $('#hosted-slot-table tbody tr');
    
    var original = $('#anchor-container').html();
    $('#temporary-row').empty().html();
    console.log("original: ", original);

    if (enable) {
        $buttons.prop('disabled', true);
        const table = document.getElementById('hosted-slot-table');
        var newRow = document.getElementById('temporary-row');
        if (!newRow) {
            newRow = document.createElement('tr');
            newRow.id = 'temporary-row';
        }
    
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
    
        const slotNameCell = document.createElement('td');
        const slotNameInput = document.createElement('input');
        slotNameInput.type = 'text';
        slotNameInput.placeholder = 'Enter Slot Name';
        slotNameInput.classList.add('form-control');
        slotNameCell.appendChild(slotNameInput);
        
        const hostNameTagCell = document.createElement('td');
        const hostNameTagInput = document.createElement('input');
        hostNameTagInput.type = 'text';
        hostNameTagInput.placeholder = 'Enter Host Name-tag';
        hostNameTagInput.classList.add('form-control');
        hostNameTagCell.appendChild(hostNameTagInput);
    
        newRow.appendChild(invitationCodeCell);
        newRow.appendChild(slotNameCell);
        newRow.appendChild(hostNameTagCell);
        newRow.appendChild(document.createElement('td'));
        newRow.appendChild(document.createElement('td'));
    
        table.querySelector('tbody').appendChild(newRow);
        
        const saveAnchor = $('<a href="#" id="save-selected-rows">Save New Host</a>');
        saveAnchor.click(function() {
            console.log("Save New Host.click()");
            if (!invitationCodeInput.value || !slotNameInput.value || !hostNameTagInput.value) {
                console.log("may be empty: {invitationCodeInput, slotNameInput, hostNameTagInput}");
                return;
            } 
            confirmNewHost(invitationCodeInput, slotNameInput, hostNameTagInput, null, original);
        });
        
        const backAnchor = $('<a href="#" id="back-to-original">Back</a>');
        backAnchor.click(function() {
            toogleNewHostMode(false);
            $('#anchor-container').html(original);
            $('#delete-multiple-host').click(function() {
                toggleDeleteMode(true);
            });
            $('#new-host').click(function() {
                toogleNewHostMode(true);
            });
        });
        
        $('#anchor-container').empty().append(saveAnchor).append(" | ").append(backAnchor);
    } else {
        console.log("else{}");
        $buttons.prop('disabled', false);
        $('#anchor-container').html(original);
    }
}