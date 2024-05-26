let addChildMode = false;

function toogleAddChildMode(enable) {
    console.log("toogleNewHostMode()");
    
    var original = $('#child-anchor-container').html();
    if (enable) {
        const table = document.getElementById('slot-children-table');
        const firstRow = table.querySelector('tbody tr:first-child');
        const firstCell = firstRow.querySelector('td:first-child'); 
        var newRow = document.getElementById('temporary-child-row');
        if (!newRow) {
            newRow = document.createElement('tr');
            newRow.id = 'temporary-row';
            
            const slotNameCell = document.createElement('td');
            const slotNameInput = document.createElement('input');
            slotNameInput.type = 'text';
            slotNameInput.placeholder = 'Enter Slot Name';
            slotNameInput.classList.add('form-control');
            slotNameCell.appendChild(slotNameInput);
            
            const isReservableCell = document.createElement('td');
            
            const radioWrapper = document.createElement('div');
            radioWrapper.classList.add('form-check');
            
            const isReservableRadioBtn = document.createElement('input');
            isReservableRadioBtn.type = 'radio';
            isReservableRadioBtn.id = 'isReservableRadio';
            isReservableRadioBtn.name = 'isReservable';
            isReservableRadioBtn.value = 'yes'; // Set the value as needed
            isReservableRadioBtn.classList.add('form-check-input');
            
            const radioLabel = document.createElement('label');
            radioLabel.setAttribute('for', 'isReservableRadio');
            radioLabel.textContent = 'Reservable'; // Set the label text as needed
            radioLabel.classList.add('form-check-label');
            
            radioWrapper.appendChild(isReservableRadioBtn);
            radioWrapper.appendChild(radioLabel);
            
            isReservableCell.appendChild(radioWrapper);
            
            const invitationCodeCell = document.createElement('td');
            const invitationCodeInput = document.createElement('input');
            invitationCodeInput.type = 'text';
            invitationCodeInput.placeholder = 'Enter Invitation Code';
            invitationCodeInput.classList.add('form-control');
            invitationCodeCell.appendChild(invitationCodeInput);
            const generateCodeAnchor = document.createElement('a');
            generateCodeAnchor.href = '#';
            generateCodeAnchor.textContent = 'Generate Code';
            generateCodeAnchor.onclick = function(e) {
                e.preventDefault();
                invitationCodeInput.value = generateRandomCode();
            };
            invitationCodeCell.appendChild(document.createElement('br'));
            invitationCodeCell.appendChild(generateCodeAnchor);
            
            newRow.appendChild(slotNameCell);
            newRow.appendChild(isReservableCell);
            newRow.appendChild(invitationCodeCell);
            newRow.appendChild(document.createElement('td'));
            newRow.appendChild(document.createElement('td'));
            newRow.appendChild(document.createElement('td'));
            
            table.querySelector('tbody').appendChild(newRow);
            
        }
    }
}