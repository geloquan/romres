
function SlotScope(slot_id) {
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

    traverse(global_slot_object);

    function extractUniqueChildren(childrenSlot) {
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

        return uniqueChildren;
    }

    if (matchedSubtree && matchedSubtree.childrenSlot) {
        matchedSubtree.childrenSlot = extractUniqueChildren(matchedSubtree.childrenSlot);
    }

    return matchedSubtree;
}
function removeFaveSlot(slot_id) {

}

function addOrUpdateButton(slotDiv, buttonText, buttonId, clickHandler) {
    let button = document.getElementById(buttonId);

    if (!button) {
        button = document.createElement('button');
        button.textContent = buttonText;
        button.classList.add('btn', 'btn-primary', 'mt-3');
        button.id = buttonId;
        button.addEventListener('click', clickHandler);

        const firstChild = slotDiv.firstChild;
        slotDiv.insertBefore(button, firstChild);

        if (buttonId === 'to-parent-btn') {
            toParentButtonAppended = true;
        } else if (buttonId === 'to-root-btn') {
            toRootButtonAppended = true;
        }
    } else {
        button.removeEventListener('click', button.clickHandler);
        button.addEventListener('click', clickHandler);
    }
}
function ParentSlot(result, parent_slot_id) {
    const slotNameElement = document.getElementById("slot-parent-name");
    slotNameElement.innerText = result.name;


    const headerContainerDiv = document.getElementById("header-container");

    const headerParentNameContainerDiv = document.getElementById('slot-parent-header-container');
    
    const duplicateSlotAnchor = document.createElement('a');
    duplicateSlotAnchor.href = "#";
    duplicateSlotAnchor.textContent = "Duplicate Slot";
    duplicateSlotAnchor.classList.add('underline');
    duplicateSlotAnchor.onclick = function() {
        duplicateSlot(result.slotId, user_id);
    };
    
    const anchorContainer = document.createElement('div');
    anchorContainer.classList.add('anchor-container');
    
    if (entity_type == 'favorites') {
    } else {
        const duplicateSlotAnchor = document.createElement('a');
        duplicateSlotAnchor.href = "#";
        duplicateSlotAnchor.textContent = "Duplicate Slot";
        duplicateSlotAnchor.classList.add('underline');
        duplicateSlotAnchor.onclick = function() {
            duplicateSlot(result.slotId, userId);
        };
        
        headerContainerDiv.appendChild(duplicateSlotAnchor);
    }

    
    headerParentNameContainerDiv.appendChild(slotNameElement)

    document.getElementById("slot-parent-id").innerText = result.slotId;
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

    const editButton = document.createElement("button");
    editButton.innerText = "Edit Slot";
    editButton.className = "btn btn-primary";

    const initialEditButtonClickHandler = function() {
        const slotNoteElement = document.getElementById("slot-parent-note");
    
        slotNoteElement.innerHTML = `<textarea id="edit-slot-note">${result.note || ''}</textarea>`;
            
        editButton.innerText = "Save";
        editButton.onclick = function() {
            const editedNote = document.getElementById("edit-slot-note").value;
            const slot_id = result.slotId;
            if (editedNote === result.note) {
                slotNoteElement.innerText = editedNote || 'No note';
                editButton.innerText = "Edit Slot";
                editButton.onclick = initialEditButtonClickHandler;
                return; 
            }
            const saveFunction = function() {
                const method = 'PUT'; 

                const requestBody = {
                    Note: editedNote,
                    UserId: userId,
                    SlotId: slot_id
                };
                console.log('Request Body:', JSON.stringify(requestBody));

                fetch(`/slot/${slot_id}/noteedit`, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(requestBody)
                })
                .then(response => {
                    if (response.ok) {
                        slotNoteElement.innerText = editedNote || 'No note';
                        editButton.innerText = "Edit Slot";
                        editButton.onclick = initialEditButtonClickHandler; // Restore initial click handler
                        $('#confirmationModal').modal('hide'); // Hide the modal after saving
                    } else {
                        console.error(`Error: Slot Edit request for ID ${slot_id} failed`);
                    }
                })
                .catch(error => {
                    console.error(`Error: Slot Edit request for ID ${slot_id} failed`);
                });
            };
            showConfirmationModal(saveFunction);
        };
    };
    editButton.onclick = initialEditButtonClickHandler;
    const slotEditTd = document.getElementById("slot-parent-edit");
    slotEditTd.innerHTML = ''; 
    slotEditTd.appendChild(editButton);
}


let toParentButtonAppended = false;
let toRootButtonAppended = false;
function processSlot(slot_id) {
    buildFaveSlotTreeTable();
    const results = SlotScope(slot_id);
    const slotDiv = document.getElementById('slot-parent-header-container');
    const anchorContainer = document.createElement('div');
    anchorContainer.classList.add('anchor-container');
    if (results) {
        ParentSlot(results, results.parentSlotId);
        ChildrenSlots(results);
        displayTable(1);
        const toRootButton = document.getElementById('to-root');
        if (!toRootButton && results.parentSlotId != null) {
            const button = document.createElement('a');
            button.textContent = `back to ${results.rootSlotName}`;
            button.classList.add('underline');
            button.id = 'to-root';
            button.onclick = function() {
                processSlot(results.parentSlotId);
            };
            anchorContainer.appendChild(button);
        } else if (results.parentSlotId != null) {
            toRootButton.onclick = function() {
                processSlot(results.parentSlotId);
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
                    processSlot(results.parentSlotId);
                };
                anchorContainer.appendChild(button);
            } else if (results.parentSlotId != null) {
                toParentButton.onclick = function() {
                    processSlot(results.parentSlotId);
                };
                toParentButton.textContent = `To Parent ${results.parentSlotName}`;
            }
        } 

        slotDiv.appendChild(anchorContainer);
    } else {
        console.log("Slot not found.");
    }
}
function ChildrenSlots(ChildrenSlotsResult) {
    if (entity_type == 'favorites') {
        const tbody = document.getElementById("slot-children");
        if (tbody) {
            while (tbody.firstChild) {
                tbody.removeChild(tbody.firstChild);
            }
        }
        if (ChildrenSlotsResult.childrenSlot) {
            ChildrenSlotsResult.childrenSlot.forEach(childSlot => {
                const row = document.createElement("tr");
                row.id = childSlot.slotId;
        
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
                const imgIcon = document.createElement("img");
                imgIcon.onload = () => {
                    // Once the image is loaded, set the dimensions
                    imgIcon.width = 24; // Set the desired width
                    imgIcon.height = 24; // Set the desired height
                };
                imgIcon.src = "../img/arrowin.svg"; 
                imgIcon.alt = "Enter Slot";
                button.appendChild(imgIcon);
                button.classList.add("btn", "btn-primary"); 
                button.addEventListener("click", () => {
                    processSlot(childSlot.slotId);
                    
                });
                buttonCell.appendChild(button);
                row.appendChild(buttonCell);
                
                tbody.appendChild(row);
        
            });
        }
    } else if (entity_type == 'hosted') {
        const tbody = document.getElementById("slot-children");
        if (tbody) {
            while (tbody.firstChild) {
                tbody.removeChild(tbody.firstChild);
            }
        }
        if (ChildrenSlotsResult.childrenSlot) {
            ChildrenSlotsResult.childrenSlot.forEach(childSlot => {
                const row = document.createElement("tr");
                row.id = childSlot.slotId;
        
                const nameCell = document.createElement("td");
                nameCell.innerText = childSlot.name;
                row.appendChild(nameCell);
        
                const isReservableCell = document.createElement("td");
                isReservableCell.innerText = childSlot.isReservable ? "Yes" : "No";
                row.appendChild(isReservableCell);
        
                const invitationCodeCell = document.createElement("td");
                invitationCodeCell.innerText = childSlot.invitationCode || 'None';
                row.appendChild(invitationCodeCell);
                
                const scheduleEntryCell = document.createElement("td");
                scheduleEntryCell.innerText = 'Calendar Entry';
                row.appendChild(scheduleEntryCell);
                
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
                const imgIcon = document.createElement("img");
                imgIcon.onload = () => {
                    // Once the image is loaded, set the dimensions
                    imgIcon.width = 24; // Set the desired width
                    imgIcon.height = 24; // Set the desired height
                };
                imgIcon.src = "../img/arrowin.svg"; 
                imgIcon.alt = "Enter Slots";
                button.appendChild(imgIcon);
                button.classList.add("btn", "btn-primary"); 
                button.addEventListener("click", () => {
                    processSlot(childSlot.slotId);
                });
                buttonCell.appendChild(button);
                row.appendChild(buttonCell);
                
                tbody.appendChild(row);
        
            });
        }
    }
}
function FavoriteSlots(slot_object) {
    const favsalottable = document.getElementById('favorites-slot-table');
    const tableBody = document.createElement('tbody');
    slot_object.slotTrees.forEach(slot_tree => {
        const row = document.createElement('tr');
        
        const slotInvitationCodeCell = document.createElement('td');
        slotInvitationCodeCell.textContent = slot_tree.rootSlotModel.invitationCode || 'None';
        row.appendChild(slotInvitationCodeCell);
        
        const nameCell = document.createElement('td');
        nameCell.textContent = slot_tree.rootSlotModel.name;
        row.appendChild(nameCell);
        
        const hostNameCell = document.createElement('td');
        hostNameCell.textContent = slot_tree.rootSlotModel.hostName;
        row.appendChild(hostNameCell);
        
        const entryCell = document.createElement("td");
        const buttonCellButton = document.createElement("button");
        buttonCellButton.innerText = `"Enter Slot" : ${slot_tree.rootSlotModel.slotId}`; 
        buttonCellButton.classList.add("btn", "btn-primary"); 
        buttonCellButton.addEventListener("click", () => {
            processSlot(slot_tree.rootSlotModel.slotId);
        });
        entryCell.appendChild(buttonCellButton);
        row.appendChild(entryCell);

        const unfaveCell = document.createElement("td");
        const unfaveBtn = document.createElement("button");
        unfaveBtn.innerText = `Unfave : ${slot_tree.rootSlotModel.slotId}`; 
        unfaveBtn.classList.add("btn", "btn-primary"); 
        unfaveBtn.addEventListener("click", () => {
            removeFaveSlot(slot_tree.rootSlotModel.slotId);
        });
        unfaveCell.appendChild(unfaveBtn);
        row.appendChild(unfaveCell);
        
        tableBody.appendChild(row);
    }); 
    favsalottable.appendChild(tableBody);
}
function SlotInit(slot_object, slot_id, user_id) {
    console.log("SlotInit() entered");
    processSlot(slot_id);
}
function HostedSlots(slot_object) {
    const hostSlotTable = document.getElementById('hosted-slot-table');
    const tableBody = document.createElement('tbody');
    slot_object.slotTrees.forEach(slot_tree => {
        const row = document.createElement('tr');
        row.id = slot_tree.rootId;
        const slotInvitationCodeCell = document.createElement('td');
        slotInvitationCodeCell.textContent = slot_tree.rootSlotModel.invitationCode || 'None';
        row.appendChild(slotInvitationCodeCell);
        
        const nameCell = document.createElement('td');
        nameCell.textContent = slot_tree.rootSlotModel.name;
        row.appendChild(nameCell);
        
        const hostNameTagCell = document.createElement('td');
        hostNameTagCell.textContent = slot_tree.rootSlotModel.hostName;
        row.appendChild(hostNameTagCell);
        
        const entryCell = document.createElement("td");
        const buttonCellButton = document.createElement("button");
        buttonCellButton.innerText = `"Enter Slot" : ${slot_tree.rootSlotModel.slotId}`; 
        buttonCellButton.classList.add("btn", "btn-primary"); 
        buttonCellButton.addEventListener("click", () => {
            processSlot(slot_tree.rootSlotModel.slotId);
        });
        entryCell.appendChild(buttonCellButton);
        row.appendChild(entryCell);

        const editCell = document.createElement("td");
        const editBtn = document.createElement("button");
        editBtn.innerText = `Edit : ${slot_tree.rootSlotModel.slotId}`; 
        editBtn.classList.add("btn", "btn-primary"); 
        editBtn.addEventListener("click", () => {
            Edit(slot_tree.rootSlotModel.slotId);
        });
        editCell.appendChild(editBtn);
        row.appendChild(editCell);
        
        tableBody.appendChild(row);
    }); 
    hostSlotTable.appendChild(tableBody);
}