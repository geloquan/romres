function buildFavoriteSlotsTable() {
    const slotDiv = document.getElementById('slot');
    slotDiv.innerHTML = '';
    slotDiv.innerHTML = '';
    const header = document.createElement('h3');
    header.textContent = 'Favorite Slots:';
    slotDiv.appendChild(header);
    const table = document.createElement('table');
    table.classList.add('table', 'table-bordered');
    const tableHead = document.createElement('thead');
    tableHead.classList.add('thead-dark');
    table.appendChild(tableHead);
    table.id = 'favorites-slot-table';
    const headerRow = document.createElement('tr');
    tableHead.appendChild(headerRow);

    const columnHeaders = ['Slot ID', 'Slot Name', 'Host Name', 'Entry'];

    columnHeaders.forEach(headerText => {
        const th = document.createElement('th');
        th.textContent = headerText;
        headerRow.appendChild(th);
    });

    const tableBody = document.createElement('tbody');
    table.appendChild(tableBody);

    slotDiv.appendChild(table);
}

function buildSlotTreeTable() {
    const slotDiv = document.getElementById('slot');
    slotDiv.innerHTML = '';
    const slotParentName = document.createElement('h3');
    slotParentName.id = 'slot-parent-name';
    slotParentName.classList.add('mb-4');
    slotDiv.appendChild(slotParentName);
    const slotParentTable = document.createElement('table');
    slotParentTable.id = 'slot-parent-table';
    slotParentTable.classList.add('table', 'table-bordered');
    const slotParentTableHead = document.createElement('thead');
    slotParentTableHead.classList.add('thead-dark');
    slotParentTable.appendChild(slotParentTableHead);
    const slotParentTableHeaderRow = document.createElement('tr');
    slotParentTableHead.appendChild(slotParentTableHeaderRow);
    const parentTableHeaders = ['Slot ID', 'Is Reservable', 'Invitation Code', 'Edge (x, y)', 'Note', 'edit'];
    parentTableHeaders.forEach(headerText => {
        const th = document.createElement('th');
        th.textContent = headerText;
        slotParentTableHeaderRow.appendChild(th);
    });

    const slotParentTableBody = document.createElement('tbody');
    const slotParentTableRow = document.createElement('tr');
    const parentTableCells = [
        'slot-parent-id',
        'slot-parent-is-reservable',
        'slot-parent-invitation-code',
        'slot-parent-edges',
        'slot-parent-note',
        'slot-parent-edit'
    ];

    parentTableCells.forEach(cellId => {
        const td = document.createElement('td');
        td.id = cellId;
        slotParentTableRow.appendChild(td);
    });

    slotParentTableBody.appendChild(slotParentTableRow);
    slotParentTable.appendChild(slotParentTableBody);

    slotDiv.appendChild(slotParentTable);

    const slotChildrenName = document.createElement('h4');
    slotChildrenName.id = 'slot-children-name';
    slotChildrenName.textContent = 'Children:';
    slotDiv.appendChild(slotChildrenName);
    const slotChildrenTable = document.createElement('table');
    slotChildrenTable.id = 'slot-children-table';
    slotChildrenTable.classList.add('table', 'table-bordered');
    const slotChildrenTableHead = document.createElement('thead');
    slotChildrenTableHead.classList.add('thead-dark');
    slotChildrenTable.appendChild(slotChildrenTableHead);
    const slotChildrenTableHeaderRow = document.createElement('tr');
    slotChildrenTableHead.appendChild(slotChildrenTableHeaderRow);
    const childrenTableHeaders = ['Slot Name', 'Is Reservable', 'Invitation Code', 'Edge (x, y)', 'Note', 'Entry'];
    childrenTableHeaders.forEach(headerText => {
        const th = document.createElement('th');
        th.textContent = headerText;
        slotChildrenTableHeaderRow.appendChild(th);
    });
    const slotChildrenTableBody = document.createElement('tbody');
    slotChildrenTableBody.id = 'slot-children';
    slotChildrenTable.appendChild(slotChildrenTableBody);

    slotDiv.appendChild(slotChildrenTable);
}