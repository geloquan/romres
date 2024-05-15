function buildRootSlotTable(header_name, column_headers, table_id) {
    console.log(`buildRootSlotTable()`);
    const slotDiv = document.getElementById('slot');
    slotDiv.innerHTML = '';

    // Create a parent div for the header
    const headerDiv = document.createElement('div');
    headerDiv.classList.add('header-container');

    // Create header
    const header = document.createElement('h3');
    header.textContent = header_name;

    // Create anchors
    const anchorContainer = document.createElement('div');
    anchorContainer.classList.add('anchor-container');
    
    const newHostAnchor = document.createElement('a');
    newHostAnchor.href = "#";
    newHostAnchor.textContent = "New Host";
    newHostAnchor.classList.add('underline');
    newHostAnchor.onclick = function() {
        addNewRow(table_id, newHostAnchor);
    };

    const unhostAnchor = document.createElement('a');
    unhostAnchor.href = "#";
    unhostAnchor.textContent = "Unhost";
    unhostAnchor.classList.add('underline');
    unhostAnchor.onclick = function() {
        // Add your unhost action here
        console.log("Unhost clicked");
    };

    anchorContainer.appendChild(newHostAnchor);
    //anchorContainer.appendChild(document.createTextNode(" | "));
    //anchorContainer.appendChild(unhostAnchor);

    headerDiv.appendChild(header);
    headerDiv.appendChild(anchorContainer);

    slotDiv.appendChild(headerDiv);

    const table = document.createElement('table');
    table.classList.add('table', 'table-bordered');
    const tableHead = document.createElement('thead');
    tableHead.classList.add('thead-dark');
    table.appendChild(tableHead);
    table.id = table_id;
    const headerRow = document.createElement('tr');
    tableHead.appendChild(headerRow);

    column_headers.forEach(headerText => {
        const th = document.createElement('th');
        th.textContent = headerText;
        headerRow.appendChild(th);
    });
    const tableBody = document.createElement('tbody');
    table.appendChild(tableBody);
    slotDiv.appendChild(table);
}
function addNewRow(table_id, newHostAnchor) {
    const table = document.getElementById(table_id);
    const newRow = document.createElement('tr');

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

    // Change Enter Slot anchor to Save anchor
    newHostAnchor.textContent = 'Save';
    newHostAnchor.onclick = function() {
        saveNewRow(newRow, invitationCodeInput, slotNameInput, hostNameTagInput);
        revertNewHostAnchor(newHostAnchor);
    };
}

function generateRandomCode() {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let code = '';
    for (let i = 0; i < 8; i++) {
        code += characters.charAt(Math.floor(Math.random() * characters.length));
    }
    return code;
}

function checkEnableButtons(invitationCodeInput, slotNameInput, hostNameTagInput, saveButton, unhostButton) {
    if (invitationCodeInput.value.trim() !== '' && slotNameInput.value.trim() !== '' && hostNameTagInput.value.trim() !== '') {
        saveButton.disabled = false;
        unhostButton.disabled = false;
    } else {
        saveButton.disabled = true;
        unhostButton.disabled = true;
    }
}
function saveNewRow(row, invitationCodeInput, slotNameInput, hostNameTagInput) {
    var newSlot = {
        hostNameTag: hostNameTagInput.value,
        invitationCode: invitationCodeInput.value,
        slotName: slotNameInput.value,
        userId: userId
    };
    console.log(newSlot);
    $.ajax({
        url: '/host/newhost',
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(newSlot),
        success: function(result) {
            console.log('Successfully saved new slot: ', result);
            var dummy_tree = createDummyRootTrees(invitationCodeInput.value, slotNameInput.value, hostNameTagInput.value, result.new_slot_id);
            console.log('dummy tree: ', dummy_tree);
            console.log('original tree struct: ', global_slot_object);
            global_slot_object.slotTrees.push(dummy_tree.slotTrees[0])
            console.log('after push struct: ', global_slot_object);
            HostedSlots(dummy_tree);
        },
        error: function() {
            alert('An error occurred while loading the content.');
        }
    });
    invitationCodeInput.disabled = true;
    slotNameInput.disabled = true;
    hostNameTagInput.disabled = true;
}
function revertNewHostAnchor(newHostAnchor) {
    console.log('revertNewHostAnchor()');
    newHostAnchor.textContent = 'New Host';
    newHostAnchor.onclick = function() {
        addNewRow(newHostAnchor.parentElement.parentElement.querySelector('table').id, newHostAnchor);
    };
}

function buildFaveSlotTreeTable() {
    console.log(`buildFaveSlotTreeTable()`);
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