let deleteMode = false;
let newHostMode = false;
const selectedRows = [];
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
                        Selected hosts has been deletted. Kindly proceed to your user-home page.
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" id="confirmButton">Go Home</button>
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
          }, 500);
    });
}

function deleteRows(slot_id_list) {
    console.log("deleteRows()");
    const table = document.getElementById('hosted-slot-table');
    for (let id of slot_id_list) {
        for (let tr of table.rows) {
            if (tr.id == id) {
                console.log("found (tr.id == id)");
                tr.remove();
            }
        }
    }
}

function sendDeleteHosts(slot_id_list) {
    console.log("sendDeleteHosts()");
    const object = {
        primarySlotId: slot_id_list
    }
    $.ajax({
        url: '/host/delete',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(object),
        success: function(result) {
        
            successDeleteHosts();
        },
        error: function() {
            alert('An error occurred while loading the content.');
        }
    });
}

function confirmDeleteHosts(slot_id_list) {
    console.log("confirmDeleteHosts()");
    if (!(slot_id_list.length >= 0)) {
        alert('Please select atleast (1) row.');
        return false;
    }

    $('#confirmationModal').remove();

    var modalHtml = `
        <div class="modal fade" id="confirmationModal" tabindex="-1" aria-labelledby="confirmationModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="confirmationModalLabel">Confirm Delete Slot</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        Are you sure you want to delete ${slot_id_list.length} of hosts?
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

    $('#confirmationModal').modal('show');

    $('#confirmationModal').on('hidden.bs.modal', function () {
        $(this).remove(); 
    });

    $('#confirmationModal').find('[data-dismiss="modal"]').on('click', function() {
        $('#confirmationModal').modal('hide');
    });

    $('#confirmButton').on('click', function() {
        sendDeleteHosts(slot_id_list);
        $('#confirmationModal').modal('hide');
    });
}
function selectRow() {
    const $row = $(this);
    const rowId = $row.attr('id');

    if ($row.hasClass('selected')) {
        $row.removeClass('selected');
        const index = selectedRows.indexOf(rowId);
        if (index > -1) {
            selectedRows.splice(index, 1);
        }
    } else {
        $row.addClass('selected');
        selectedRows.push(rowId);
    }

    console.log('Selected rows:', selectedRows);
}
function toggleDeleteMode(enable) {
    console.log("toggleDeleteMode()");
    const $buttons = $('#hosted-slot-table .btn');
    const $rows = $('#hosted-slot-table tbody tr');
    
    var original = $('#anchor-container').html();
    console.log("original: ", original);

    if (enable) {
        $buttons.prop('disabled', true);
        
        const saveAnchor = $('<a href="#" id="save-selected-rows">Delete Rows</a>');
        saveAnchor.click(function() {
            console.log("Delete Rows.click()");
            if (selectedRows.length >= 0) {
                confirmDeleteHosts(selectedRows);
                return;
            } 
            console.log("may be empty: {selectedRows}");
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