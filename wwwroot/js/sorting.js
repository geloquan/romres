function sortTable(direction) {
    console.log('rows', row_copy);
    row_copy = [...row_copy];
    
    row_copy.sort((a, b) => {
        const cellA = a.getElementsByTagName('td')[0].innerText;
        const cellB = b.getElementsByTagName('td')[0].innerText;

        if (direction === 'asc') {
            return cellA.localeCompare(cellB);
        } else if (direction === 'desc') {
            return cellB.localeCompare(cellA);
        } else {
            throw new Error('Invalid direction. Use "asc" or "desc".');
        }
    });

    row_copy.forEach(row => table_copy.appendChild(row));
    
    displayTable(1);
    console.log('sorted rows', row_copy);
}