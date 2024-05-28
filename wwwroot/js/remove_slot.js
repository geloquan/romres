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
