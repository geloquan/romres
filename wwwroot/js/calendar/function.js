function writeProperty(key, value) {
    const div = document.createElement('div');
    div.classList.add('cal-property');

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
