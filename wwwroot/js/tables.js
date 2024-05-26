function buildRootSlotTable(header_name, column_headers, table_id) {
    console.log(`buildRootSlotTable()`);
    const slotDiv = document.getElementById('slot');
    slotDiv.innerHTML = '';

    const headerDiv = document.createElement('div');
    headerDiv.classList.add('header-container');

    const header = document.createElement('h3');
    header.textContent = header_name;

    const anchorContainer = document.createElement('div');
    anchorContainer.classList.add('anchor-container');
    anchorContainer.id = "anchor-container";
    switch (header_name) {
        case "Hosted":
            const newHostAnchor = document.createElement('a');
            newHostAnchor.id = "new-host";
            newHostAnchor.href = "#";
            newHostAnchor.textContent = "New Host";
            newHostAnchor.classList.add('underline');
            newHostAnchor.onclick = function() {
                newHostMode = !newHostMode;
                //addNewRow(table_id, newHostAnchor);
                toogleNewHostMode(newHostMode);
            };
            const deleteHostAnchor = document.createElement('a');
            deleteHostAnchor.id = "delete-multiple-host"
            deleteHostAnchor.href = "#";
            deleteHostAnchor.textContent = "Delete multiple hosts";
            deleteHostAnchor.classList.add('underline');
            deleteHostAnchor.onclick = function() {
                deleteMode = !deleteMode;
                toggleDeleteMode(deleteMode);
            };
            //const duplicateHost = document.createElement('a');
            //duplicateHost.href = "#";
            //duplicateHost.textContent = "Duplicate Host";
            //duplicateHost.classList.add('underline');
            //duplicateHost.onclick = function() {
            //    const saveFunction = function() {
            //        const method = 'PUT'; 
            //
            //        const requestBody = {
            //        };
            //        console.log('Request Body:', JSON.stringify(requestBody));
            //
            //        fetch(`/host/${host_id}/duplicate/parent/slot/${slot_id}`, {
            //            method: method,
            //            headers: {
            //                'Content-Type': 'application/json'
            //            },
            //            body: JSON.stringify(requestBody)
            //        })
            //        .then(response => {
            //            if (response.ok) {
            //                
            //                $('#confirmationModal').modal('hide'); 
            //            } else {
            //                console.error(`Error: Slot Edit request for ID ${slot_id} failed`);
            //            }
            //        })
            //        .catch(error => {
            //            console.error(`Error: Slot Edit request for ID ${slot_id} failed`);
            //        });
            //    };
            //    showConfirmationModal(saveFunction, null, "Are you sure?", "This action will duplicate this slot with its succeeding children.", "Cancel", "Gow!");
            //};
            //
            anchorContainer.appendChild(deleteHostAnchor);
            anchorContainer.appendChild(document.createTextNode(" | "));
            anchorContainer.appendChild(newHostAnchor);
            break;
        case "Favorites":
            const findFaveAnchor = document.createElement('a');
            findFaveAnchor.href = "#";
            findFaveAnchor.textContent = "Find Host";
            findFaveAnchor.classList.add('underline');
            findFaveAnchor.onclick = function() {
                const modalDialog = document.createElement('div');
                modalDialog.classList.add('modal', 'fade');
                modalDialog.id = 'findSlotModal';
                modalDialog.tabIndex = '-1';
                modalDialog.setAttribute('role', 'dialog');
                modalDialog.setAttribute('aria-labelledby', 'findSlotModalLabel');
                modalDialog.setAttribute('aria-hidden', 'true');

                modalDialog.innerHTML = `
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title" id="findSlotModalLabel">Find Slot</h5>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div class="modal-body">
                                <p>Enter Invitation Code:</p>
                                <input type="text" id="searchSlotInput" class="form-control" placeholder="Enter code...">
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" id="cancelButton">Cancel</button>
                                <button type="button" class="btn btn-primary" id="searchButton">Search</button>
                            </div>
                        </div>
                    </div>
                `;

                document.body.appendChild(modalDialog);

                $('#findSlotModal').modal('show');

                const searchButton = modalDialog.querySelector('#searchButton');
                searchButton.addEventListener('click', function() {
                    const invitation_code_input = $('#searchSlotInput').val();
                    $.ajax({
                        url: `/slot/${invitation_code_input}/exists`,
                        method: 'GET',
                        contentType: 'application/json',
                        success: function(data) {
                            console.log("Slot exists: ", data);
                        },
                        error: function(xhr, status, error) {
                            console.error('Error:', error);
                        },
                        complete: function() {
                            $('#findSlotModal').modal('hide'); // Hide the modal after executing search
                        }
                    });
                    $('#findSlotModal').modal('hide'); // Hide the modal after executing search
                });

                const cancelButton = modalDialog.querySelector('#cancelButton');
                cancelButton.addEventListener('click', function() {
                    $('#findSlotModal').modal('hide'); // Hide the modal on cancel
                });

                const closeButton = modalDialog.querySelector('.close');
                closeButton.addEventListener('click', function() {
                    $('#findSlotModal').modal('hide'); // Hide the modal on close (X)
                });

                // Prevent default anchor behavior
                return false;
            };
            anchorContainer.appendChild(findFaveAnchor);

        default:
            break;
    }
    const unhostAnchor = document.createElement('a');
    unhostAnchor.href = "#";
    unhostAnchor.textContent = "Unhost";
    unhostAnchor.classList.add('underline');
    unhostAnchor.onclick = function() {
        // Add your unhost action here
        console.log("Unhost clicked");
    };

    
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
    
    const original_anchor_container = $('#anchor-container').clone();

    const cancelNewHostAnchor = document.createElement('a');
    cancelNewHostAnchor.onclick = function() {
        const table = document.getElementById('hosted-slot-table');
        if (table.rows.length > 0) {
            table.deleteRow(0); // delete the first row
        }
        $('#anchor-container').html(original_anchor_container.html());
        revertNewHostAnchor(newHostAnchor, cancelNewHostAnchor);
    };

    newHostAnchor.textContent = 'Save New Host';
    newHostAnchor.onclick = function() {
        console.log('newHostAnchor.onclick()');
        if (!invitationCodeInput.value || !slotNameInput.value || !hostNameTagInput.value) {
            console.log("may be empty: {invitationCodeInput, slotNameInput, hostNameTagInput}");
            return;
        } 
        var res = confirmNewHost(invitationCodeInput.value, slotNameInput.value, hostNameTagInput.value);
        if (res) {
            saveNewRow(null, invitationCodeInput, slotNameInput, hostNameTagInput);
            revertNewHostAnchor(newHostAnchor, cancelNewHostAnchor);
            const temporaryRow = document.getElementById('temporary-row');
                table.deleteRow(0); // delete the first row
        } else {
        }
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
            var dummy_tree = createDummyRootTrees(newSlot.invitationCode, newSlot.slotName, newSlot.hostNameTag, result.new_slot_id);
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
    invitationCodeInput.disabled = true;
    slotNameInput.disabled = true;
    hostNameTagInput.disabled = true;
}

function revertNewHostAnchor(newHostAnchor, cancelNewHostAnchor) {
    console.log('revertNewHostAnchor()');
    newHostAnchor.textContent = 'New Host';
    newHostAnchor.onclick = function() {
        addNewRow('hosted-slot-table', newHostAnchor);
    };
    cancelNewHostAnchor.onclick = function() {
        addNewRow('hosted-slot-table', newHostAnchor);
    };
}

function buildFaveSlotTreeTable(tree) {
    console.log(`buildFaveSlotTreeTable()`);

    const slotDiv = document.getElementById('slot');
    slotDiv.innerHTML = '';

    const slotParentDiv = document.createElement('div');
    slotParentDiv.classList.add('header-container');
    slotParentDiv.id = 'header-container';
    
    const slotParentName = document.createElement('h3');
    slotParentName.id = 'slot-parent-name';
    slotParentName.classList.add('mb-4', 'text-center');
    

    const slotParentNameDiv = document.createElement('div');
    slotParentNameDiv.classList.add('header-container', 'd-flex', 'justify-content-center');
    slotParentNameDiv.id = 'slot-parent-header-container';

    slotParentNameDiv.appendChild(slotParentName);
    slotParentDiv.appendChild(slotParentNameDiv);
    slotDiv.appendChild(slotParentDiv);

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

    const headerDiv = document.createElement('div');
    headerDiv.classList.add('header-container');

    const slotChildrenName = document.createElement('h4');
    slotChildrenName.id = 'slot-children-name';
    slotChildrenName.textContent = 'Children:';

    const anchorContainer = document.createElement('div');
    anchorContainer.classList.add('anchor-container');
    anchorContainer.id = 'child-anchor-container';

    if (entity_type == 'favorites') {
    } else {
        const addSlotChildAnchor = document.createElement('a');
        addSlotChildAnchor.href = "#";
        addSlotChildAnchor.textContent = "Add child";
        addSlotChildAnchor.classList.add('underline');
        addSlotChildAnchor.onclick = function() {
            addChildMode = !addChildMode;
            toogleAddChildMode(addChildMode);
        };
        //anchorContainer.appendChild(duplicateHost);
        //anchorContainer.appendChild(document.createTextNode(" | "));
        anchorContainer.appendChild(addSlotChildAnchor);
    }

    //headerDiv.appendChild(header);
    headerDiv.appendChild(slotChildrenName);
    headerDiv.appendChild(anchorContainer);

    slotDiv.appendChild(headerDiv);

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
        if (headerText == 'Slot Name') {
            th.innerHTML = `${headerText} 
                <button id='' onclick="sortTable('asc')">▲</button>
                <button id='' onclick="sortTable('desc')">▼</button>`;
        }
        slotChildrenTableHeaderRow.appendChild(th);
    });
    const slotChildrenTableBody = document.createElement('tbody');
    const emptyTBody = document.createElement('tbody');
    slotChildrenTableBody.id = 'slot-children';
    slotChildrenTable.appendChild(emptyTBody);
    slotChildrenTable.appendChild(slotChildrenTableBody);

    slotDiv.appendChild(slotChildrenTable);
    
    const paginationDiv = document.createElement('div');
    paginationDiv.id = 'pagination-controls';

    const nav = document.createElement('nav');
    const ul = document.createElement('ul');
    ul.className = 'pagination justify-content-center';

    const prevLi = document.createElement('li');
    prevLi.className = 'page-item';
    const prevBtn = document.createElement('button');
    prevBtn.className = 'page-link';
    prevBtn.id = 'prev';
    prevBtn.innerText = 'Previous';
    prevBtn.onclick = prevPage;
    prevLi.appendChild(prevBtn);
    
    const pageInfoLi = document.createElement('li');
    pageInfoLi.className = 'page-item disabled';
    const pageInfoSpan = document.createElement('span');
    pageInfoSpan.className = 'page-link';
    pageInfoSpan.id = 'page-info';
    pageInfoLi.appendChild(pageInfoSpan);

    const nextLi = document.createElement('li');
    nextLi.className = 'page-item';
    const nextBtn = document.createElement('button');
    nextBtn.className = 'page-link';
    nextBtn.id = 'next';
    nextBtn.innerText = 'Next';
    nextBtn.onclick = nextPage;
    nextLi.appendChild(nextBtn);

    ul.appendChild(prevLi);
    ul.appendChild(pageInfoLi);
    ul.appendChild(nextLi);
    nav.appendChild(ul);
    paginationDiv.appendChild(nav);
    
    slotDiv.appendChild(paginationDiv);
    
}