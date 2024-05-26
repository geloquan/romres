
let currentPage = 1;
const rowsPerPage = 5;
let totalPages = 1;
var row_copy = [];
var table_copy;


function displayTable(page) {
    const table = document.getElementById('slot-children');
    const rows = table.getElementsByTagName('tr');
    console.log('displayTable() rows', rows);
    row_copy = rows;
    table_copy = table;
    var page_info = `0 / 0`;
    if (rows.length == 0) {
        document.getElementById('page-info').innerText = page_info;
    } else {
        totalPages = Math.ceil(rows.length / rowsPerPage);
        
        if (page < 1) page = 1;
        if (page > totalPages) page = totalPages;
        
        for (let i = 0; i < rows.length; i++) {
            rows[i].style.display = 'none';
        }
        
        for (let i = (page - 1) * rowsPerPage; i < (page * rowsPerPage) && i < rows.length; i++) {
            rows[i].style.display = '';
        }
        document.getElementById('prev').disabled = (page === 1);
        document.getElementById('next').disabled = (page === totalPages);
        
        document.getElementById('page-info').innerText = `${page} / ${totalPages}`;
    }
    
}

function prevPage() {
    if (currentPage > 1) {
        currentPage--;
        displayTable(currentPage);
    }
}

function nextPage() {
    const table = document.getElementById('slot-children');
    const rows = table.getElementsByTagName('tr');
    const totalPages = Math.ceil(rows.length / rowsPerPage);
    
    if (currentPage < totalPages) {
        currentPage++;
        displayTable(currentPage);
    }
}