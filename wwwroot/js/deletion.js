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

    if (enable) {
        $buttons.prop('disabled', true);
        $('#delete-multipl-host').text('Back');
        $rows.addClass('pointer').on('click', selectRow);
    } else {
        $buttons.prop('disabled', false);
        $('#delete-multipl-host').text('Delete multiple hosts');
        $rows.removeClass('pointer selected').off('click', selectRow);
        selectedRows.length = 0; // Clear the selected rows
    }
}