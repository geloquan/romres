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
function Init(slot_object, slot_id) {
    const result = SlotScope(slot_object, slot_id);
    if (result) {
        const slotNameElement = document.getElementById("slot-name");
        slotNameElement.innerText = result.name;
        document.getElementById("slot-id").innerText = result.slotId;
        document.getElementById("slot-is-reservable").innerText = result.isReservable ? "Yes" : "No";
        document.getElementById("slot-invitation-code").innerText = result.invitationCode || 'None'; 
        document.getElementById("slot-note").innerText = result.note || 'No note'; 
        let edgesText = '';
        console.log("result.isReservable:", result.isReservable);
        console.log("result.invitationCode:", result.invitationCode);
        if (result.edges && Array.isArray(result.edges)) {
            edgesText = result.edges.map(coord => `(${coord.x}, ${coord.y})`).join(', ');
        }
        document.getElementById("slot-edges").innerText = edgesText;
        const tbody = document.getElementById("slot-children");

        // Clear existing rows in tbody (optional)
        tbody.innerHTML = '';

        // Loop through each child slot and append a new row to the tbody
        result.childrenSlot.forEach(childSlot => {
            const row = document.createElement("tr"); // Create a new table row

            // Create table cells (td) for each property of the child slot
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

            // Append the row to the tbody
            tbody.appendChild(row);
        });
        console.log("Result:", result);
    } else {
        console.log("Slot not found.");
    }
}