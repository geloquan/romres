function generateRandomString() {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    for (let i = 0; i < 5; i++) {
        result += characters.charAt(Math.floor(Math.random() * characters.length));
    }
    return result;
}
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
                            var property_box = writeProperty(property.calendarPropertyId, property.calendarPropertyKey, property.calendarPropertyValue);

                            tr_property.appendChild(property_box);  
                            tr_list.push(tr_property);
                        });

                        const overlay = document.createElement('div');
                        const overlay_id = 'overlay - ' + tds[0].innerText + '-' + th_elements[index].innerText;
                        overlay.id = overlay_id;

                        const body = document.createElement('div');
                        const content = document.createElement('div');
                        content.id = 'calendar - ' + elem.calendarId;

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
                        const mouseoverHandler = (overlay_ide) => (e) => {
							e.preventDefault();
							const overlay = document.getElementById(overlay_ide);
							const td = document.getElementById(td_id);
							console.log('overlay ', overlay);
							console.log('td ',td);
							const tr_list_td = tr_list;
							
							if (overlay && td) {
								tr_list_td.forEach(eleme => {
									content.appendChild(eleme);
								});
							
								if (!td.contains(event.relatedTarget) && !overlay.contains(event.relatedTarget)) {
									td.innerHTML = '';
								}
							
								overlay.classList.add('overlay');
								overlay.style.display = 'block';
							
								const overlayWidth = overlay.offsetWidth;
								const overlayHeight = overlay.offsetHeight;
								const arrowHeight = arrow ? arrow.offsetHeight : 0; // Ensure arrow exists before accessing offsetHeight
							
								let left = e.clientX;
								let top = e.clientY;
							
								if (left + overlayWidth > window.innerWidth) {
									left = window.innerWidth - overlayWidth - 10; // 10px for margin
									if (arrow) arrow.style.left = `${e.clientX - left}px`;
								} else {
									if (arrow) arrow.style.left = '10px';
								}
							
								if (top + overlayHeight + arrowHeight > window.innerHeight) {
									top = window.innerHeight - overlayHeight - arrowHeight - 10; // 10px for margin
									if (arrow) {
										arrow.style.top = `${overlayHeight}px`; // Move arrow to the bottom
										arrow.style.transform = 'rotate(180deg)';
									}
								} else {
									if (arrow) {
										arrow.style.top = '-10px';
										arrow.style.transform = 'rotate(0deg)';
									}
								}
							
								if (arrow) overlay.appendChild(arrow);
							
								overlay.style.left = `${left - 19}px`;
								overlay.style.top = `${top + 10}px`;
							} else {
								console.error('Overlay or td element not found.');
							}
							
						};

                        const mouseleaveHandler = (overlay_ide) => (e) => {
							console.log('mouseleaveHandler');
                            const td = document.getElementById(td_id);
                            const overlay = document.getElementById(overlay_ide);
							console.log('overlay ', overlay);
							console.log('td ',td);
							if (td && overlay) {
								if (!td.contains(e.relatedTarget) && !overlay.contains(e.relatedTarget)) {
									overlay.style.display = 'none';
								} 
							} else {
								console.error('td or overlay not found');
							}
                        };
                        
                        td.addEventListener('mouseover', mouseoverHandler(overlay_id));
                        td.addEventListener('mouseleave', mouseleaveHandler(overlay_id));
                        overlay.addEventListener('mouseleave', mouseleaveHandler(overlay_id));
                        const edit_onclick = function() {
                            console.log(`edit_btn ${tds[0].innerText} - ${th_elements[index].innerText}`);
                            const overlay_div = document.getElementById(overlay_id);
                            overlay_div.removeEventListener('mouseleave', mouseleaveHandler(overlay_id));
                            const overlay_clone = document.getElementById(overlay_id).cloneNode(true);
                            const key_divs = overlay_div.querySelectorAll('.key');
                            const value_divs = overlay_div.querySelectorAll('.value');
                            const property_to_delete = [];
                            const property_to_add = [];
                            
                            const properties = overlay_div.querySelectorAll('.cal-property');
                            properties.forEach(property => {
                                const property_id = property.id.split(' ')[2];
                                const button_container = document.createElement("div");
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
                                    const property_div = document.getElementById(property.id);
                                    property_div.remove();
                                    property_to_delete.push(property_id);
                                    console.log('removing property...', property_id);
                                });
                                
                                button_container.classList.add("d-flex", "justify-content-center", "align-items-center");
                                button_container.appendChild(button);

                                property.appendChild(button_container);

                            });

                            const contentElement = overlay_div.querySelector(".content");
                            const add_div = document.createElement("div");
                            const add_button = document.createElement("button");
                            const add_icon = document.createElement("img");
                            add_icon.onload = () => {
                                add_icon.width = 24; 
                                add_icon.height = 24;
                            };
                            add_icon.src = "/img/add.svg"; 
                            add_icon.alt = "add";

                            add_button.appendChild(add_icon);
                            add_button.classList.add("btn", "btn-block", "d-flex", "justify-content-center", "align-items-center", "h-100", "w-100", "p-3", "border-0", "bg-transparent");
                            add_button.addEventListener("click", () => {
                                const overlay_div = document.getElementById(overlay_id);
                                const overlay_rand_id = 'temp - ' + generateRandomString();
                                const content_div = overlay_div.querySelector('.content');
                                const tr = document.createElement('tr');
                                const div = document.createElement('div');
                                div.id = overlay_rand_id;
                                div.classList.add('cal-property');
                                
                                const p_key = document.createElement('div');
                                p_key.classList.add('key');
                                const p_key_input = document.createElement('input');
                                p_key_input.type = 'text';
                                p_key.appendChild(p_key_input);

                                const colonText = '\u00A0\u00A0:\u00A0\u00A0'; // Non-breaking spaces around the colon
                                const colonElement = document.createElement('span');
                                colonElement.innerText = colonText;

                                const p_value = document.createElement('div');
                                p_value.classList.add('value');
                                const p_value_input = document.createElement('input');
                                p_value_input.type = 'text';
                                p_value.appendChild(p_value_input);
                                const button_container = document.createElement("div");
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
                                    const property_div = document.getElementById(overlay_rand_id);
                                    property_div.remove();
                                    property_to_delete.push(overlay_rand_id);
                                    console.log('removing property...', overlay_rand_id);
                                });

                                button_container.classList.add("d-flex", "justify-content-center", "align-items-center");
                                button_container.appendChild(button);

                                div.appendChild(p_key); 
                                div.appendChild(colonElement);
                                div.appendChild(p_value); 
                                div.appendChild(button_container); 
                                tr.appendChild(div);
                                content_div.append(tr);
                            });
                            
                            add_div.appendChild(add_button);
                            add_div.classList.add("w-100", "h-100");
                            
                            contentElement.insertAdjacentElement('afterend', add_div);
                            
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
                            
                            console.log('td: ', td);
                            console.log('td_id: ', td_id);
                            const save_button = document.createElement('button');
                            save_button.innerText = 'Save';
                            save_button.addEventListener('click', (e) => {
                                console.log('save-button');
                                const overlay_div = document.getElementById(overlay_id);
                                confirmEditedSlot(null, null, null, overlay_div, mouseoverHandler, mouseleaveHandler, td_id, duplicated_buttons, property_to_delete);
                            });
                            
                            const cancel_button = document.createElement('button');
                            cancel_button.innerText = 'Cancel';
                            cancel_button.addEventListener('click', (e) => {
                                console.log('cancel-button');
								const overlay_id_copy = overlay_id;
                                console.log('overlay_id_copy ', overlay_id_copy);
                                const overlay_dive = document.getElementById(overlay_id_copy);
                                const overlay_clone_copy = overlay_clone;
								overlay_clone_copy.id = overlay_id_copy;
								overlay_clone_copy.style.display = 'none';
                                console.log('overlay_clone_copy ', overlay_clone_copy);
                                console.log('overlay_dive', overlay_dive);
                                const parent_div = document.getElementById(td_id);
                                if (overlay_dive && overlay_dive.parentNode) {
                                    overlay_dive.parentNode.removeChild(overlay_dive);
                                    console.log('overlay_dive removed');
                                } else {
                                    console.log('overlay_dive is not a child of overlay_div');
                                }
                                document.body.appendChild(overlay_clone_copy);
								console.log('parent_div ', parent_div);
                                overlay_clone_copy.style.display = 'none';
                                const td_ = document.getElementById(td_id);
                                td_.addEventListener('mouseover', mouseoverHandler(overlay_id_copy));
                                td_.addEventListener('mouseleave', mouseleaveHandler(overlay_id));
                                overlay_clone_copy.addEventListener('mouseleave', mouseleaveHandler(overlay_id_copy));
                                const buttons_div = overlay_clone_copy.getElementsByClassName('overlay-buttons')[0];
                                console.log('buttons_div ', buttons_div);
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
                                value_div.innerHTML = '';
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
						overlay.style.display = 'none';
                        document.body.appendChild(overlay); 
                    } else {
						const new_calendar_id = 'new-calendar - ' + tds[0].innerText + '-' + th_elements[index].innerText;
						const createBtnListener = (e) => {
							console.log('createBtnListener');
							if (!td.querySelector('.btn') && !td.classList.contains('has-property')) {
								const newButton = document.createElement('button');
								newButton.id = new_calendar_id;
								newButton.textContent = 'create';
								newButton.classList.add('btn', 'btn-primary', 'btn-sm');
								newButton.addEventListener('click', () => {
									console.log('Button Clicked');
								});
								const pasteButton = document.createElement('button');
								pasteButton.textContent = 'paste';
								pasteButton.disabled = true;
								pasteButton.classList.add('btn', 'btn-primary', 'btn-sm');
								pasteButton.addEventListener('click', () => {
									console.log('pasteButton Clicked');
								});
		
								td.appendChild(newButton);
								td.appendChild(pasteButton);
							}
						};
						const mouseleaveCreateBtn = (e) => {
						  console.log('mouseleaveCreateBtn');
						  const button = td.querySelector('.btn');
						  if (button) {
							button.remove();
						  }
						};
						
						// Remove the existing event listener before adding the new one
						td.addEventListener('mouseover', createBtnListener);
						td.addEventListener('mouseleave', mouseleaveCreateBtn);
						
					}
                });
            } 
        });
    });
}