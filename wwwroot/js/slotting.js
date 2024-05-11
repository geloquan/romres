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
function ParentSlot(result, user_id) {
    console.log("ParentSlot() entered");
    const slotNameElement = document.getElementById("slot-parent-name");
    slotNameElement.innerText = result.name;
    
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
    editButton.onclick = function() {
        console.log("editButton.onclick = function() entered");
        const slotNoteElement = document.getElementById("slot-parent-note");
    
        slotNoteElement.innerHTML = `<textarea id="edit-slot-note">${result.note || ''}</textarea>`;
            

        editButton.innerText = "Save";
        editButton.onclick = function() {
            console.log("editButton.onclick = function() entered");
            const editedNote = document.getElementById("edit-slot-note").value;
    
            const slot_id = result.slotId;
    
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
                } else {
                    console.error(`Error: Slot Edit request for ID ${slot_id} failed`);
                }
            })
            .catch(error => {
                console.error(`Error: Slot Edit request for ID ${slot_id} failed`);
            });
        };
    
        const initialEditButtonClickHandler = function() {
            console.log("initialEditButtonClickHandler() entered");
            slotNoteElement.innerText = result.note || 'No note';

            editButton.innerText = "Edit Slot";
            editButton.onclick = editButtonClickHandler; // Restore initial click handler
        };
    };
    const slotEditTd = document.getElementById("slot-parent-edit");
    slotEditTd.innerHTML = ''; 
    slotEditTd.appendChild(editButton);
}
let toParentButtonAppended = false;
let toRootButtonAppended = false;
function processSlot(slot_id) {
    console.log("processSlot() entered");
    buildSlotTreeTable();
    const results = SlotScope(slot_id);
    console.log('results: ', results);
    const slotDiv = document.getElementById('slot');
    if (results) {
        ParentSlot(results);
        ChildrenSlots(results);
        
        const existingButton = document.getElementById('to-parent');
        if (!existingButton && results.parentSlotId != null) {
            const button = document.createElement('button');
            button.textContent = `to parent ${results.parentSlotId}`;
            button.id = 'to-parent';
            button.onclick = function() {
                console.log("button.onclick = function() entered");
                processSlot(results.parentSlotId);
            };
            if (slotDiv.firstChild) {
                slotDiv.insertBefore(button, slotDiv.firstChild);
            } else {
                slotDiv.appendChild(button); // If no child nodes, simply append the button
            }
        } else if (results.parentSlotId != null) {
            existingButton.onclick = function() {
                console.log("existingButton.onclick = function() entered");
                processSlot(results.parentSlotId);
            };
            existingButton.textContent = `to parent ${results.parentSlotId}`;
        }

    } else {
        console.log("Slot not found.");
    }
}
function ChildrenSlots(ChildrenSlotsResult) {
    console.log("entered ChildrenSlots()");
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
function FavoriteSlots(slot_object) {
    const favsalottable = document.getElementById('favorites-slot-table');
    const tableBody = document.createElement('tbody');
    slot_object.slotTrees.forEach(slot_tree => {
        const row = document.createElement('tr');
        
        const slotIdCell = document.createElement('td');
        slotIdCell.textContent = slot_tree.rootSlotModel.slotId;
        row.appendChild(slotIdCell);
        
        const nameCell = document.createElement('td');
        nameCell.textContent = slot_tree.rootSlotModel.name;
        row.appendChild(nameCell);
        
        const hostNameCell = document.createElement('td');
        hostNameCell.textContent = slot_tree.rootSlotModel.hostName;
        row.appendChild(hostNameCell);

        
        const buttonCell = document.createElement("td");
        const button = document.createElement("button");
        button.innerText = `"Enter Slot" : ${slot_tree.rootSlotModel.slotId}`; 
        button.classList.add("btn", "btn-primary"); 
        button.addEventListener("click", () => {
            processSlot(slot_tree.rootSlotModel.slotId);
        });
        buttonCell.appendChild(button);
        row.appendChild(buttonCell);
        
        tableBody.appendChild(row);
    }); 
    favsalottable.appendChild(tableBody);
}
function SlotInit(slot_object, slot_id, user_id) {
    console.log("SlotInit() entered");
    processSlot(slot_id);
}
function SlotTreeInit(slot_object) {
    console.log("SlotInit() entered");
    FavoriteSlots(slot_object);
}