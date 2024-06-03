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

function supplyFunction(schedule_object) {
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
                    //console.log(`${elem.row_label} == ${tds[0].innerText} && ${elem.column_label} == ${th_elements[index].innerText}`)
                    if (elem.rowLabel == tds[0].innerText && elem.columnLabel == th_elements[index].innerText) {
                        td.classList.add('has-property');
                        console.log('1');
                        var tr_list = [];
                        console.log('trList: ', tr_list);
                        elem.properties.forEach(property => {
                            const tr_property = document.createElement('tr');
                            var property_box = writeProperty(property.calendarPropertyKey, property.calendarPropertyValue);

                            tr_property.appendChild(property_box);  
                            console.log('property_box: ', property_box);
                            console.log('tr_property: ', tr_property);
                            tr_list.push(tr_property);
                        });
                        console.log('trList: ', tr_list);
                        td.addEventListener('mouseover', (e) => {
                            console.log('hovered');
                            e.preventDefault();
                            tr_list.forEach(eleme => {
                                td.appendChild(eleme);
                            });
                            if (!td.contains(event.target) &&!overlay.contains(event.target)) {
                                td.innerHTML = '';
                            }
                        });
                        td.addEventListener('mouseleave', (e) => {
                            // Check if the mouse is leaving the td element and not entering the overlay
                            if (!td.contains(e.relatedTarget)) {
                                td.innerHTML = ''; // Clear the td contents
                            }
                        });
                    }
                });
                if (!td.classList.contains('has-property')) {
                    console.log('2');
                    td.addEventListener('contextmenu', (e) => {
                      e.preventDefault();
    
                      const overlay = document.createElement('div');
                      overlay.className = 'overlay';
                      overlay.innerHTML = '<button class="btn btn-primary">CREATE</button>';
                      td.appendChild(overlay);
                      
                      const button = overlay.querySelector('button');
                      button.addEventListener('click', () => {
                        console.log('CREATE button clicked! ', tds[0].innerText, " + ", th_elements[index].innerText);
                      });
                  
                      document.addEventListener('mouseover', (event) => {
                        if (!td.contains(event.target) &&!overlay.contains(event.target)) {
                          overlay.remove();
                        }
                      });
                    });
                }
            }
        });
    });
}