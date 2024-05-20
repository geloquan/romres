let deleteMode = false;
const selectedRows = [];

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
        
        const saveAnchor = $('<a href="#" id="save-selected-rows">Save Selected Rows</a>');
        saveAnchor.click(function() {
            console.log("saveAnchor.click()");
        });
        
        const backAnchor = $('<a href="#" id="back-to-original">Back</a>');
        backAnchor.click(function() {
            toggleDeleteMode(false);
            $('#anchor-container').html(original);
            $('#delete-multiple-host').click(function() {
                toggleDeleteMode(true);
            });
            $('#new-host').click(function() {
                addNewRow("hosted-slot-table", $('#new-host'));
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