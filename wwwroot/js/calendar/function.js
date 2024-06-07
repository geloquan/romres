function writeProperty(id, key, value) {
    const div = document.createElement('div');
    div.classList.add('cal-property');
    div.id = 'property - ' + id;

    const p_key = document.createElement('div');
    p_key.classList.add('key');
    p_key.innerText = key;

    const colonText = '\u00A0\u00A0:\u00A0\u00A0'; // Non-breaking spaces around the colon
    const colonElement = document.createElement('span');
    colonElement.innerText = colonText;

    const p_value = document.createElement('div');
    p_value.classList.add('value');
    p_value.innerText = value;

    div.appendChild(p_key); 
    div.appendChild(colonElement);
    div.appendChild(p_value); 
    return div;
}

function sendEditedCalendar(to_edit, overlay_div, mouseoverHandler, mouseleaveHandler, td_id, duplicated_buttons) {
    console.log('sendEditedCalendar()');
    
    $.ajax({
        url: '/calendar/edit',
        type: 'PATCH',
        contentType: 'application/json',
        data: JSON.stringify(to_edit),
        success: function(result) {
            console.log('sendEditedCalendar() OK');
            
            const key_divs = overlay_div.querySelectorAll('.key');
            const value_divs = overlay_div.querySelectorAll('.value');
            overlay_div.style.display = 'none';
            overlay_div.addEventListener('mouseleave', mouseleaveHandler);
                            
            const properties = overlay_div.querySelectorAll('.cal-property');
            properties.forEach(property => {
                const button = document.createElement("button");
                const remove_icon = document.createElement("img");
                remove_icon.onload = () => {
                    remove_icon.width = 24; 
                    remove_icon.height = 24;
                };
                remove_icon.src = "/img/minus.svg"; 
                remove_icon.alt = "remove";
                button.appendChild(remove_icon);
                button.classList.add("btn-no-bg"); 
                button.addEventListener("click", () => {
                    console.log('removing property... ');
                });
                property.appendChild(button);
            });

            const td = document.getElementById(td_id);
            td.addEventListener('mouseover', mouseoverHandler);
            td.addEventListener('mouseleave', mouseleaveHandler);
            key_divs.forEach(value_div => {
                const inputElement = value_div.querySelector('input');
                if (inputElement) {
                    const inner_text = inputElement.value;
                    value_div.innerHTML = '';
                    value_div.innerText = inner_text;
                    console.log("inputElement.textContent: ", inner_text);
                }
            })
            value_divs.forEach(value_div => {
                const inputElement = value_div.querySelector('input');
                if (inputElement) {
                    const inner_text = inputElement.value;
                    value_div.innerHTML = '';
                    value_div.innerText = inner_text;
                    console.log("inputElement.textContent: ", inner_text);
                }
            })
            
            const buttons_div = overlay_div.getElementsByClassName('overlay-buttons')[0];
            console.log('buttons_div ', buttons_div);
            while (buttons_div.firstChild) {
                buttons_div.removeChild(buttons_div.firstChild);
            }
            duplicated_buttons.forEach(duplicated_button => {
                console.log('duplicated_button ', duplicated_button.onclick);
                buttons_div.appendChild(duplicated_button);
            });
        },
        error: function() {
            alert('An error occurred while loading the content.');
        }
    });
}

function confirmEditedSlot(calendar_id, slot_id, calendar_properties, overlay_div, mouseoverHandler, mouseleaveHandler, td_id, duplicated_buttons, property_to_delete) {
    console.log('confirmEditedSlot()');
    var calendarDataPropertyModel = [];
    console.log('calendarDataPropertyModel: ', calendarDataPropertyModel);

    const content_ = overlay_div.getElementsByClassName('content');
    const calendar_id_ = content_[0].id.split(' ')[2];

    Array.from(content_).forEach(content => {
        const properties = content.getElementsByClassName('cal-property');
        Array.from(properties).forEach(property => {
            console.log('properties', properties);
            console.log('property', property);
            const keyElement = property.querySelector('.key input');
            const valueElement = property.querySelector('.value input');
            const property_id = property.id.split(' ')[2];

            if (keyElement && valueElement) {
                const key_val = keyElement.value;
                const value_val = valueElement.value;
                console.log('key_val', key_val);
                console.log('value_val', value_val);
                
                calendarDataPropertyModel.push({
                    id: property_id,
                    calendar_id: calendar_id_,
                    key: key_val,
                    value: value_val
                });
            }
        });
    });
    var requestBody = {
        calendar_properties: calendarDataPropertyModel,
        property_to_delete: property_to_delete
    };
    console.log('requestBody: ', requestBody);
    
    $('#confirmationModal').remove();

    var modalHtml = `
        <div class="modal fade" id="confirmationModal" tabindex="-1" aria-labelledby="confirmationModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="confirmationModalLabel">Are you sure?</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        Are you sure you want to save changes?
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <button type="button" class="btn btn-primary" id="confirmButton">Confirm</button>
                    </div>
                </div>
            </div>
        </div>
    `;

    $('body').append(modalHtml);

    $('#confirmationModal').modal('show');

    $('#confirmationModal').on('hidden.bs.modal', function () {
        $(this).remove(); 
    });

    $('#confirmationModal').find('[data-dismiss="modal"]').on('click', function() {
        console.log('confirmationModal');
        $('#confirmationModal').modal('hide');
    });

    $('#confirmButton').on('click', function() {
        console.log('confirmButton');
        sendEditedCalendar(requestBody, overlay_div, mouseoverHandler, mouseleaveHandler, td_id, duplicated_buttons);
        $('#confirmationModal').modal('hide');
    });
}