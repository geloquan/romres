function traverse_calendar() {

}
function initCalendar() {
    const tbl_calendar = document.getElementById('calendar');
    switch (preference.repetition_pattern) {
        case 'weekly':
            tbl_calendar.appendChild(theadCalendar([
                'Sunday',
                'Monday',
                'Tuesday',
                'Wednesday',
                'Thursday',
                'Friday',
                'Saturday'
            ]))
            tbl_calendar.appendChild(tbodyCalendar());
            break;
        default:
            tbl_calendar.innerText = 'preferences does not match the formatting...';
            break;
    }
}
function theadCalendar(headers) {
    let thead = document.createElement('thead');
    thead.id = 'calendar-thead';
    thead.classList.add('table-blue'); 
    let tr = document.createElement('tr');
    tr.appendChild(document.createElement('th'));
    headers.forEach(element => {
        let th = document.createElement('th');
        th.innerText = element;
        tr.appendChild(th);
    });
    thead.appendChild(tr);
    return thead;
}
function tbodyCalendar() {
    let tbody = document.createElement('tbody');
    tbody.id = 'calendar-tbody';
    let startHour = 0;
    let startMinute = 0;

    for (let i = 0; i < 48; i++) {
        let tr = document.createElement('tr');

        let startTime = `${String(startHour).padStart(2, '0')}:${String(startMinute).padStart(2, '0')}`;

        startMinute += 30;
        if (startMinute >= 60) {
            startMinute = 0;
            startHour += 1;
        }

        let endTime = `${String(startHour).padStart(2, '0')}:${String(startMinute).padStart(2, '0')}`;

        let td = document.createElement('td');
        //if (startMinute.toString().slice(-3) == '0') {
        //    td.textContent = '...';
        //    td.style.fontSize = '10px';
        //} else {
            td.textContent = `${startTime} - ${endTime}`;
        //}
        tr.appendChild(td);

        for (let j = 1; j < 8; j++) {
            let emptyTd = document.createElement('td');
            tr.appendChild(emptyTd);
        }

        tbody.appendChild(tr);
    }

    return tbody;

    
}
function singlePlot(row_label, column_label, key, value) {
    const thead_tr = document.getElementById('calendar-thead').getElementsByTagName('tr')[0]; 
    const th_elements = thead_tr.getElementsByTagName('th');

    const tbody_tr = document.getElementById('calendar-tbody').getElementsByTagName('tr');

    thead_tr.setAttribute('role', 'row');
    Array.prototype.forEach.call(th_elements, (th, index) => {
        th.setAttribute('role', 'columnheader');
        th.setAttribute('scope', 'col');
    });
    Array.prototype.forEach.call(tbody_tr, elem => {
        const tds = elem.getElementsByTagName('td');
        Array.prototype.forEach.call(tds, (td, index) => {
            if (!td.innerText) {
                if (row_label == tds[0].innerText && column_label == th_elements[index].innerText) {
                    td.classList.add('has-property');
                    const td_id = tds[0].innerText + '-' + th_elements[index].innerText;
                    td.id =  td_id;
                    console.log('1');
                    var tr_list = [];
                    console.log('trList: ', tr_list);
                    elem.properties.forEach(property => {
                        const tr_property = document.createElement('tr');
                        var property_box = writeProperty(key, value);

                        tr_property.appendChild(property_box);  
                        console.log('property_box: ', property_box);
                        console.log('tr_property: ', tr_property);
                        tr_list.push(tr_property);
                    });
                    console.log('trList: ', tr_list);
                    td.addEventListener('mouseover', (e) => {
                        e.preventDefault();
                        tr_list.forEach(eleme => {
                            document.querySelector('#overlay .content').appendChild(eleme);
                        });
                        if (!td.contains(event.target) &&!overlay.contains(event.target)) {
                            td.innerHTML = '';
                        }
                        
                        overlay.style.display = 'block';
                        const overlayWidth = overlay.offsetWidth;
                        const overlayHeight = overlay.offsetHeight;
                        const arrow = overlay.querySelector('.arrow');
                        const arrowHeight = arrow.offsetHeight;

                        let left = e.clientX;
                        let top = e.clientY;

                        if (left + overlayWidth > window.innerWidth) {
                            left = window.innerWidth - overlayWidth - 10; // 10px for margin
                            arrow.style.left = `${e.clientX - left}px`;
                        } else {
                            arrow.style.left = '10px';
                        } 

                        if (top + overlayHeight + arrowHeight > window.innerHeight) {
                            top = window.innerHeight - overlayHeight - arrowHeight - 10; // 10px for margin
                            arrow.style.top = `${overlayHeight}px`; // Move arrow to the bottom
                            arrow.style.transform = 'rotate(180deg)';
                        } else {
                            arrow.style.top = '-10px';
                            arrow.style.transform = 'rotate(0deg)';
                        }
                        
                        overlay.style.left = `${left}px`;
                        overlay.style.top = `${top}px`;
                        
                    });
                    td.addEventListener('mouseleave', (e) => {
                        if (!td.contains(e.relatedTarget) && !overlay.contains(e.relatedTarget)) {
                            td.innerHTML = ''; 
                            overlay.style.display = 'none';
                        }
                    });
                    overlay.addEventListener('mouseleave', (e) => {
                        if (!td.contains(e.relatedTarget) && !overlay.contains(e.relatedTarget)) {
                            td.innerHTML = ''; 
                            overlay.style.display = 'none';
                        }
                    });
                } 
            }
        });

    });
    
}
function supplyFunction() {
    const thead_tr = document.getElementById('calendar-thead').getElementsByTagName('tr')[0]; 
    const th_elements = thead_tr.getElementsByTagName('th');

    const tbody_tr = document.getElementById('calendar-tbody').getElementsByTagName('tr');

    thead_tr.setAttribute('role', 'row');
    Array.prototype.forEach.call(th_elements, (th, index) => {
        th.setAttribute('role', 'columnheader');
        th.setAttribute('scope', 'col');
    });
    Array.prototype.forEach.call(tbody_tr, elem => {
        const tds = elem.getElementsByTagName('td');
        Array.prototype.forEach.call(tds, (td, index) => {
            if (!td.innerText) {
                    data.calendarDataModel.forEach(elem => {
                        if (elem.rowLabel == tds[0].innerText && elem.columnLabel == th_elements[index].innerText) {
                        td.classList.add('has-property');
                        const td_id = tds[0].innerText + '-' + th_elements[index].innerText;
                        td.id =  td_id;
                        var tr_list = [];
                        elem.properties.forEach(property => {
                            const tr_property = document.createElement('tr');
                            var property_box = writeProperty(property.calendarPropertyKey, property.calendarPropertyValue);

                            tr_property.appendChild(property_box);  
                            tr_list.push(tr_property);
                        });

                        const overlay = document.createElement('div');
                        const overlay_id = 'overlay-' + tds[0].innerText + '-' + th_elements[index].innerText;
                        overlay.id = overlay_id;

                        const body = document.createElement('div');
                        const content = document.createElement('div');

                        const buttons = document.createElement('div');
                        buttons.classList.add('overlay-buttons');
                        
                        const arrow = document.createElement('div');
                        content.classList.add('content');
                        arrow.classList.add('arrow');

                        const copy_btn = document.createElement('button');
                        copy_btn.innerText = 'COPY';
                        copy_btn.addEventListener('click', (e) => {
                            console.log(`copy_btn ${tds[0].innerText} - ${th_elements[index].innerText}`);
                        });

                        const delete_btn = document.createElement('button');
                        delete_btn.innerText = 'DELETE';
                        delete_btn.addEventListener('click', (e) => {
                            console.log(`delete_btn ${tds[0].innerText} - ${th_elements[index].innerText}`);
                        });
                        const mouseoverHandler = (e) => {
                            console.log('mouseoverHandler');
                          e.preventDefault();
                          const tr_list_td = tr_list;
                          const overlay = document.getElementById(overlay_id);
                          tr_list_td.forEach(eleme => {
                            console.log('eleme ', eleme);
                            content.appendChild(eleme);
                          });
                          if (!td.contains(event.target) &&!overlay.contains(event.target)) {
                            td.innerHTML = '';
                          }
                          overlay.classList.add('overlay');
                          overlay.style.display = 'block';
                          const overlayWidth = overlay.offsetWidth;
                          const overlayHeight = overlay.offsetHeight;
                          const arrowHeight = arrow.offsetHeight;
                          let left = e.clientX;
                          let top = e.clientY;
                          if (left + overlayWidth > window.innerWidth) {
                            left = window.innerWidth - overlayWidth - 10; // 10px for margin
                            arrow.style.left = `${e.clientX - left}px`;
                          } else {
                            arrow.style.left = '10px';
                          } 
                          if (top + overlayHeight + arrowHeight > window.innerHeight) {
                            top = window.innerHeight - overlayHeight - arrowHeight - 10; // 10px for margin
                            arrow.style.top = `${overlayHeight}px`; // Move arrow to the bottom
                            arrow.style.transform = 'rotate(180deg)';
                          } else {
                            arrow.style.top = '-10px';
                            arrow.style.transform = 'rotate(0deg)';
                          }
                          
                          overlay.appendChild(arrow);
                          
                          overlay.style.left = `${left-19}px`;
                          overlay.style.top = `${top+10}px`;

                        };

                        const mouseleaveHandler = (e) => {
                            console.log('mouseleaveHandler');
                            const overlay = document.getElementById(overlay_id);
                              if (!td.contains(e.relatedTarget) && !overlay.contains(e.relatedTarget)) {
                                console.log('mouseleaveHandler true');
                                overlay.style.display = 'none';
                              }
                        };
                        
                        td.addEventListener('mouseover', mouseoverHandler);
                        td.addEventListener('mouseleave', mouseleaveHandler);
                        overlay.addEventListener('mouseleave', mouseleaveHandler);
                        const edit_onclick = function() {
                            console.log(`edit_btn ${tds[0].innerText} - ${th_elements[index].innerText}`);
                            const overlay_div = document.getElementById(overlay_id);
                            overlay_div.removeEventListener('mouseleave', mouseleaveHandler);
                            const overlay_clone = document.getElementById(overlay_id).cloneNode(true);
                            const key_divs = overlay_div.querySelectorAll('.key');
                            const value_divs = overlay_div.querySelectorAll('.value');
                            
                            const original_buttons = Array.from(overlay_div.getElementsByClassName('overlay-buttons')[0].children);
                            const duplicated_buttons = original_buttons.map(button => {
                                const clone = button.cloneNode(true);
                                clone.onclick = button.onclick;
                                return clone;
                            });
                            console.log('duplicated_buttons: ', duplicated_buttons);
                            duplicated_buttons.forEach(duplicated_button => {
                                console.log('duplicated_button ', duplicated_button.onclick);
                            });
                            const buttons_div = overlay_div.getElementsByClassName('overlay-buttons')[0];
                            while (buttons_div.firstChild) {
                                buttons_div.removeChild(buttons_div.firstChild);
                            }
                            
                            const save_button = document.createElement('button');
                            save_button.innerText = 'Save';
                            save_button.addEventListener('click', (e) => {
                                console.log('save-button');
                            });
                            
                            const cancel_button = document.createElement('button');
                            cancel_button.innerText = 'Cancel';
                            cancel_button.addEventListener('click', (e) => {
                                console.log('cancel-button');
                                const overlay_dive = document.getElementById(overlay_id);
                                const parent_div = overlay_dive.parentElement;
                                parent_div.removeChild(overlay_dive);
                                parent_div.appendChild(overlay_clone);
                                overlay_clone.style.display = 'none';
                                overlay_dive.addEventListener('mouseover', mouseoverHandler);
                                overlay_dive.addEventListener('mouseleave', mouseleaveHandler);
                                const buttons_div = overlay_clone.getElementsByClassName('overlay-buttons')[0];
                                while (buttons_div.firstChild) {
                                    buttons_div.removeChild(buttons_div.firstChild);
                                }
                                duplicated_buttons.forEach(duplicated_button => {
                                    console.log('duplicated_button ', duplicated_button.onclick);
                                    buttons_div.appendChild(duplicated_button);
                                });
                            });
                            
                            buttons_div.appendChild(save_button);
                            buttons_div.appendChild(cancel_button);
                            
                            key_divs.forEach(key_div => {
                                const input = document.createElement('input');
                                input.type = 'text';
                                input.value = key_div.innerText;
                                key_div.innerHTML = ''; 
                                key_div.appendChild(input);
                            });
                            
                            value_divs.forEach(value_div => {
                                const input = document.createElement('input');
                                input.type = 'text';
                                input.value = value_div.innerText;
                                value_div.innerHTML = ''; // Clear the current content
                                value_div.appendChild(input);
                            });
                        };
                        
                        const edit_btn = document.createElement('button');
                        edit_btn.innerText = 'EDIT';
                        
                        edit_btn.onclick = edit_onclick;

                        overlay.appendChild(arrow);
                        body.appendChild(content);
                        buttons.appendChild(edit_btn);
                        buttons.appendChild(copy_btn);
                        buttons.appendChild(delete_btn);
                        body.appendChild(buttons);
                        overlay.appendChild(body);
                        document.body.appendChild(overlay); 
                    } 
                });
            }
        });
    });
}