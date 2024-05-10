function SlotScope(slot_object, slot_id) {
    let matchedSubtree = null;
    function traverse(obj) {
        if (typeof obj !== 'object' || obj === null) {
            return; 
        }

        if (obj.hasOwnProperty('slotId') && obj.slotId === slot_id) {
            matchedSubtree = obj; 
        }

        if (obj.hasOwnProperty('parentSlotId') && obj.parentSlotId === slot_id) {
            if (!matchedSubtree) {
                matchedSubtree = {}; 
                Object.assign(matchedSubtree, obj); 
                matchedSubtree.childrenSlot = [];
            } else if (!matchedSubtree.childrenSlot) {
                matchedSubtree.childrenSlot = []; 
            }
            matchedSubtree.childrenSlot.push(obj);
        }
        for (let prop in obj) {
            if (obj.hasOwnProperty(prop)) {
                traverse(obj[prop]);
            }
        }
    }
    traverse(slot_object);
    function extractChildrenSlotId(childrenSlot) {
        if (!childrenSlot || !Array.isArray(childrenSlot)) {
            return []; 
        }
        const childrenSlotId = [];
        for (let slot of childrenSlot) {
            if (slot.hasOwnProperty('slotId')) {
                childrenSlotId.push(slot.slotId); 
            }
        }
        return childrenSlotId;
    }
    if (matchedSubtree && matchedSubtree.childrenSlot) {
        matchedSubtree.childrenSlotId = extractChildrenSlotId(matchedSubtree.childrenSlot);
    }
    return matchedSubtree;
}
function ParentSlot(result) {
    const slotNameElement = document.getElementById("slot-parent-name");
    slotNameElement.innerText = result.name;
    
    document.getElementById("slot-parent-id").innerText = result.slotId;
    document.getElementById("slot-parent-is-reservable").innerText = result.isReservable ? "Yes" : "No";
    document.getElementById("slot-parent-invitation-code").innerText = result.invitationCode || 'None'; 
    document.getElementById("slot-parent-note").innerText = result.note || 'No note'; 
    let edgesText = '';
    console.log("result.isReservable:", result.isReservable);
    console.log("result.invitationCode:", result.invitationCode);
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
        //const slotNameElement = document.getElementById("slot-parent-name");
        //const slotIsReservableElement = document.getElementById("slot-parent-is-reservable");
        //const slotInvitationCodeElement = document.getElementById("slot-parent-invitation-code");
        const slotNoteElement = document.getElementById("slot-parent-note");
    
        //slotNameElement.innerHTML = `<input type="text" value="${result.name}" id="edit-slot-name">`;
        //slotIsReservableElement.innerHTML = `<input class="form-check-input mt-0" type="checkbox" ${result.isReservable ? 'checked' : ''} id="edit-slot-reservable">`;
        //slotInvitationCodeElement.innerHTML = `<input type="text" value="${result.invitationCode || ''}" id="edit-slot-invitation">`;
        slotNoteElement.innerHTML = `<textarea id="edit-slot-note">${result.note || ''}</textarea>`;
    
        editButton.innerText = "Save";
        editButton.onclick = function() {
            //const editedName = document.getElementById("edit-slot-name").value;
            //const editedReservable = document.getElementById("edit-slot-reservable").checked;
            //const editedInvitationCode = document.getElementById("edit-slot-invitation").value;
            const editedNote = document.getElementById("edit-slot-note").value;
    
            const id = result.slotId;
    
            const method = 'PUT'; // Use PUT method for updating existing slot
    
            fetch(`/slot/${id}/noteedit`, {
                method: method,
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    note: editedNote
                })
            })
            .then(response => {
                if (response.ok) {
                    console.log(`Action: Slot Edit for ID ${id} completed successfully`);
                    //slotNameElement.innerText = editedName;
                    //slotIsReservableElement.innerText = editedReservable ? "Yes" : "No";
                    //slotInvitationCodeElement.innerText = editedInvitationCode || 'None';
                    slotNoteElement.innerText = editedNote || 'No note';
                    // Restore button text and behavior
                    editButton.innerText = "Edit Slot";
                    editButton.onclick = initialEditButtonClickHandler; // Restore initial click handler
                } else {
                    console.error(`Error: Slot Edit request for ID ${id} failed`);
                }
            })
            .catch(error => {
                console.error(`Error: Slot Edit request for ID ${id} failed`);
            });
        };
    
        const initialEditButtonClickHandler = function() {
            //slotNameElement.innerText = result.name;
            //slotIsReservableElement.innerText = result.isReservable ? "Yes" : "No";
            //slotInvitationCodeElement.innerText = result.invitationCode || 'None';
            slotNoteElement.innerText = result.note || 'No note';

            editButton.innerText = "Edit Slot";
            editButton.onclick = editButtonClickHandler; // Restore initial click handler
        };
    };
    const slotEditTd = document.getElementById("slot-parent-edit");
    slotEditTd.innerHTML = ''; 
    slotEditTd.appendChild(editButton);
}
function ChildrenSlots(result) {
    const tbody = document.getElementById("slot-children");
    result.childrenSlot.forEach(childSlot => {
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
        tbody.appendChild(row);

    });
}
function Init(slot_object, slot_id) {
    const result = SlotScope(slot_object, slot_id);
    if (result) {
        ParentSlot(result);
        ChildrenSlots(result);
        console.log("Result:", result);
    } else {
        console.log("Slot not found.");
    }
}