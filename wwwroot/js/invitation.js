function buildInvitationBase() {
    const slotDiv = document.getElementById('slot');
    slotDiv.innerHTML = '';

    const headerDiv = document.createElement('div');
    headerDiv.classList.add('header-container');
    
    const header = document.createElement('h3');
    header.textContent = header_name;

    headerDiv.appendChild(header);
    
    slotDiv.appendChild(headerDiv);

    const table = document.createElement('table');
    table.classList.add('table', 'table-bordered');
    const tableHead = document.createElement('thead');
    tableHead.classList.add('thead-dark');
    table.appendChild(tableHead);
    table.id = table_id;
    const headerRow = document.createElement('tr');
    tableHead.appendChild(headerRow);

    const column_headers = ["Slot name", "Host"]
    column_headers.forEach(headerText => {
        const th = document.createElement('th');
        th.textContent = headerText;
        headerRow.appendChild(th);
    });
    const tableBody = document.createElement('tbody');
    table.appendChild(tableBody);
    slotDiv.appendChild(table);
}