function deleteRow(table_id, slot_id) {
    const table = document.getElementById(table_id);
    const rows = table.rows;
    for (let i = 0; i < rows.length; i++) {
        const tr = rows[i];
        if (tr.id == slot_id) {
            table.deleteRow(i);
            break; 
        }
    }
}
function deleteSlot(slot_id, slotName) {
    // Recursive function to remove elements based on slot_id or slotName
    function removeSlotByIdOrName(obj, slot_id, slotName) {
        if (Array.isArray(obj)) {
            // Filter the array to exclude values or objects with matching slot_id or slotName
            return obj.filter(item => {
                if (typeof item === 'object' && item !== null) {
                    // Check if the object itself has a slotId or slotName property
                    if (item.slotId === slot_id || item.slotName === slotName) {
                        return false;
                    }
                    // Recursively check if the item contains arrays
                    for (let key in item) {
                        if (Array.isArray(item[key])) {
                            item[key] = removeSlotByIdOrName(item[key], slot_id, slotName);
                        }
                    }
                    return true;
                }
                // Directly check if the item matches slot_id or slotName
                return item !== slot_id && item !== slotName;
            });
        } else if (typeof obj === 'object' && obj !== null) {
            // Recursively check all properties of the object
            for (let key in obj) {
                if (Array.isArray(obj[key])) {
                    obj[key] = removeSlotByIdOrName(obj[key], slot_id, slotName);
                } else if (obj[key] === slot_id || obj[key] === slotName) {
                    // Remove the key if its value matches slot_id or slotName
                    delete obj[key];
                }
            }
        }
        return obj;
    }

    // Check if global_slot_object is an array or an object
    if (Array.isArray(global_slot_object) || (typeof global_slot_object === 'object' && global_slot_object !== null)) {
        global_slot_object = removeSlotByIdOrName(global_slot_object, slot_id, slotName);
    } else {
        console.error("global_slot_object is not an array or object");
    }
}