function SlotScope(slot_id) {
    console.log("SlotScope() entered");
    let matchedSubtree = null;

    function traverse(obj) {
        console.log("traverse() entered");
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
function removeFaveSlot(slot_id) {

}

function addOrUpdateButton(slotDiv, buttonText, buttonId, clickHandler) {
    console.log("addOrUpdateButton() entered");
    let button = document.getElementById(buttonId);

    if (!button) {
        console.log("new button");
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
    console.log("ParentSlot() entered");
    const slotNameElement = document.getElementById("slot-parent-name");
    slotNameElement.innerText = result.name;


    const headerContainerDiv = document.getElementById("header-container");

    const headerParentNameContainerDiv = document.getElementById('slot-parent-header-container');
    
    const toParentAnchor = document.createElement('a');
    toParentAnchor.href = "#";
    toParentAnchor.textContent = "Duplicate Slot";
    toParentAnchor.classList.add('underline');
    toParentAnchor.onclick = function() {
        processSlot(result.rootSlotModel.slotId);
    };

    const anchorContainer = document.createElement('div');
    anchorContainer.classList.add('anchor-container');

    const duplicateSlotAnchor = document.createElement('a');
    duplicateSlotAnchor.href = "#";
    duplicateSlotAnchor.textContent = "Duplicate Slot";
    duplicateSlotAnchor.classList.add('underline');
    duplicateSlotAnchor.onclick = function() {
        duplicateSlot(result.slotId, parent_slot_id);
    };

    headerContainerDiv.appendChild(duplicateSlotAnchor);
    
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
        console.log("editButton.onclick = function() entered");
        const slotNoteElement = document.getElementById("slot-parent-note");
    
        slotNoteElement.innerHTML = `<textarea id="edit-slot-note">${result.note || ''}</textarea>`;
            
        console.log("slot_id: ", result.slotId);
        console.log("userId: ", user_id);

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
    console.log("processSlot() entered");
    buildFaveSlotTreeTable();
    const results = SlotScope(slot_id);
    console.log('results: ', results);
    const slotDiv = document.getElementById('slot-parent-header-container');
    const anchorContainer = document.createElement('div');
    anchorContainer.classList.add('anchor-container');
    if (results) {
        ParentSlot(results, results.parentSlotId);
        ChildrenSlots(results);
        console.log("resres: ", results);
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
        console.log("entered ChildrenSlots(favorites)");
        console.log("ChildrenSlotsResult length: ", ChildrenSlotsResult);
        const tbody = document.getElementById("slot-children");
        if (tbody) {
            while (tbody.firstChild) {
                tbody.removeChild(tbody.firstChild);
            }
        }
        if (ChildrenSlotsResult.childrenSlot) {
            ChildrenSlotsResult.childrenSlot.forEach(childSlot => {
                const row = document.createElement("tr");
        
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
                    processSlot(childSlot.slotId);
                });
                buttonCell.appendChild(button);
                row.appendChild(buttonCell);
                
                tbody.appendChild(row);
        
            });
        }
    } else if (entity_type == 'hosted') {
        console.log("entered ChildrenSlots(hosted)");
        console.log("ChildrenSlotsResult length: ", ChildrenSlotsResult);
        const tbody = document.getElementById("slot-children");
        if (tbody) {
            while (tbody.firstChild) {
                tbody.removeChild(tbody.firstChild);
            }
        }
        if (ChildrenSlotsResult.childrenSlot) {
            ChildrenSlotsResult.childrenSlot.forEach(childSlot => {
                const row = document.createElement("tr");
        
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
    console.log("sloted objects: ", slot_object);
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

        const unhostCell = document.createElement("td");
        const unhostBtn = document.createElement("button");
        unhostBtn.innerText = `Unhost : ${slot_tree.rootSlotModel.slotId}`; 
        unhostBtn.classList.add("btn", "btn-primary"); 
        unhostBtn.addEventListener("click", () => {
            removeHostSlot(slot_tree.rootSlotModel.slotId);
        });
        unhostCell.appendChild(unhostBtn);
        row.appendChild(unhostCell);
        
        tableBody.appendChild(row);
    }); 
    hostSlotTable.appendChild(tableBody);
}