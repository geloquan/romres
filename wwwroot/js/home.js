function SlotScope(slot_id, slot_object) {
    console.log("SlotScope() entered");
    let matchedSubtree = null;

    function traverse(obj) {
        if (typeof obj !== 'object' || obj === null) {
            return;
        }

        if (obj.hasOwnProperty('slotId') && obj.slotId === slot_id) {
            if (!matchedSubtree) {
                matchedSubtree = { ...obj, childrenSlot: [] }; // Clone the matched subtree
            }
        }

        if (obj.hasOwnProperty('parentSlotId') && obj.parentSlotId === slot_id) {
            if (!matchedSubtree) {
                matchedSubtree = { ...obj, childrenSlot: [] }; // Clone the matched subtree
            }
            const existingChild = matchedSubtree.childrenSlot.find(child => child.slotId === obj.slotId);
            if (!existingChild) {
                matchedSubtree.childrenSlot.push(obj);
            }
        }

        for (let prop in obj) {
            if (obj.hasOwnProperty(prop)) {
                traverse(obj[prop]);
            }
        }
    }

    traverse(slot_object);
    console.log("Matched Subtree:", matchedSubtree);

    function extractUniqueChildren(childrenSlot) {
        console.log("extractUniqueChildren() entered");
        if (!childrenSlot || !Array.isArray(childrenSlot)) {
            return [];
        }

        const uniqueChildren = [];

        childrenSlot.forEach(child => {
            const existingChild = uniqueChildren.find(c => c.slotId === child.slotId);
            if (!existingChild) {
                uniqueChildren.push(child);
            }
        });

        console.log("Unique Children:", uniqueChildren);
        return uniqueChildren;
    }

    if (matchedSubtree && matchedSubtree.childrenSlot) {
        matchedSubtree.childrenSlot = extractUniqueChildren(matchedSubtree.childrenSlot);
    }

    console.log("Final Matched Subtree with Unique Children:", matchedSubtree);
    return matchedSubtree;
}

function buildSearchSlotTreeTable() {
    console.log(`buildSearchSlotTreeTable()`);

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

    const parentTableHeaders = ['Is Reservable', 'Invitation Code', 'Edge (x, y)', 'Note'];
    parentTableHeaders.forEach(headerText => {
        const th = document.createElement('th');
        th.textContent = headerText;
        slotParentTableHeaderRow.appendChild(th);
    });

    const slotParentTableBody = document.createElement('tbody');
    const slotParentTableRow = document.createElement('tr');
    const parentTableCells = [
        'slot-parent-is-reservable',
        'slot-parent-invitation-code',
        'slot-parent-edges',
        'slot-parent-note',
    ];

    parentTableCells.forEach(cellId => {
        const td = document.createElement('td');
        td.id = cellId;
        slotParentTableRow.appendChild(td);
    });

    slotParentTableBody.appendChild(slotParentTableRow);
    slotParentTableBody.classList.add("custom-row"); 
    
    slotParentTable.appendChild(slotParentTableBody);

    slotDiv.appendChild(slotParentTable);

    const headerDiv = document.createElement('div');
    headerDiv.classList.add('header-container');

    const slotChildrenName = document.createElement('h4');
    slotChildrenName.id = 'slot-children-name';
    slotChildrenName.textContent = 'Children:';

    const anchorContainer = document.createElement('div');
    anchorContainer.classList.add('anchor-container');

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
        slotChildrenTableHeaderRow.appendChild(th);
    });
    const slotChildrenTableBody = document.createElement('tbody');
    slotChildrenTableBody.id = 'slot-children';
    slotChildrenTable.appendChild(slotChildrenTableBody);

    slotDiv.appendChild(slotChildrenTable);
}

function processSlot(slot_id, slot) {
    console.log("processSlot() entered");
    const slotDiv = document.createElement('div');
    slotDiv.id = "slot";
    
    $('main').append(slotDiv);
    
    buildSearchSlotTreeTable();
    const results = SlotScope(slot_id, slot);
    console.log('results: ', results);
    const parentHeaderDiv = document.getElementById('slot-parent-header-container');
    const anchorContainer = document.createElement('div');
    anchorContainer.classList.add('anchor-container');
    if (results) {
        ParentSlot(results, results.parentSlotId);
        ChildrenSlots(results, slot);
        console.log("resres: ", results);
        const toRootButton = document.getElementById('to-root');
        if (!toRootButton && results.parentSlotId != null) {
            const button = document.createElement('a');
            button.textContent = `back to ${results.rootSlotName}`;
            button.classList.add('underline');
            button.id = 'to-root';
            button.onclick = function() {
                processSlot(results.parentSlotId, slot);
            };
            anchorContainer.appendChild(button);
        } else if (results.parentSlotId != null) {
            toRootButton.onclick = function() {
                processSlot(results.parentSlotId, slot);
            };
            toRootButton.textContent = `To Parent ${results.parentSlotName}`;
        }
        if (results.rootSlotName != results.parentSlotName) {
            const toParentButton = document.getElementById('to-parent');
            if (!toParentButton && results.parentSlotId != null) {
                anchorContainer.appendChild(document.createTextNode(" | "));
                const button = document.createElement('a');
                button.textContent = `To Parent ${results.parentSlotName}`;
                button.classList.add('underline');
                button.id = 'to-parent';
                button.onclick = function() {
                    processSlot(results.parentSlotId, slot);
                };
                anchorContainer.appendChild(button);
            } else if (results.parentSlotId != null) {
                toParentButton.onclick = function() {
                    processSlot(results.parentSlotId, slot);
                };
                toParentButton.textContent = `To Parent ${results.parentSlotName}`;
            }
        } 

        parentHeaderDiv.appendChild(anchorContainer);
    } else {
        console.log("Slot not found.");
    }
}

function ParentSlot(result, parent_slot_id) {
    const slotNameElement = document.getElementById("slot-parent-name");
    slotNameElement.innerText = result.name;
    
    const headerContainerDiv = document.getElementById("header-container");
    
    const headerParentNameContainerDiv = document.getElementById('slot-parent-header-container');
    
    headerParentNameContainerDiv.appendChild(slotNameElement);
    document.getElementById("slot-parent-is-reservable").innerText = result.isReservable ? "Yes" : "No";
    document.getElementById("slot-parent-invitation-code").innerText = result.invitationCode || 'None'; 
    document.getElementById("slot-parent-note").innerText = result.note || 'No note'; 
    let edgesText = '';
    if (result.edges && Array.isArray(result.edges)) {
        edgesText = result.edges.map(coord => `(${coord.x}, ${coord.y})`).join(', ');
    }
    document.getElementById("slot-parent-edges").innerText = edgesText;
    const tbody = document.getElementById("slot-children");

    tbody.innerHTML = '';
}

function ChildrenSlots(ChildrenSlotsResult, slot) {
    const tbody = document.getElementById("slot-children");
    
    if (tbody) {
        while (tbody.firstChild) {
            tbody.removeChild(tbody.firstChild);
        }
    }
    
    ChildrenSlotsResult.childrenSlot.forEach(childSlot => {
        const row = document.createElement("tr");
        row.id = childSlot.slotId;
        row.classList.add("custom-row"); 
        
        const nameCell = document.createElement("td");
        nameCell.innerText = childSlot.name;
        row.appendChild(nameCell);
        
        const isReservableCell = document.createElement("td");
        isReservableCell.innerText = childSlot.isReservable ? "Yes" : "No";
        row.appendChild(isReservableCell);
        
        const invitationCodeCell = document.createElement("td");
        invitationCodeCell.innerText = childSlot.invitationCode || 'None';
        row.appendChild(invitationCodeCell);
        
        let childEdgesText = '';
        if (childSlot.edges && Array.isArray(childSlot.edges)) {
            childEdgesText = childSlot.edges.map(coord => `(${coord.x}, ${coord.y})`).join(', ');
        }
        const edgesCell = document.createElement("td");
        edgesCell.innerText = childEdgesText;
        row.appendChild(edgesCell);
        
        const noteCell = document.createElement("td");
        noteCell.innerText = childSlot.note || 'No note';
        row.appendChild(noteCell);
        
        const buttonCell = document.createElement("td");
        const button = document.createElement("button");
        button.innerText = "Enter Slot"; 
        button.classList.add("btn", "btn-primary"); 
        button.addEventListener("click", () => {
            processSlot(childSlot.slotId, slot);
        });
        buttonCell.appendChild(button);
        row.appendChild(buttonCell);
        
        tbody.appendChild(row);
    });
}

