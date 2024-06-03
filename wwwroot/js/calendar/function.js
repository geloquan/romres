function writeProperty(key, value) {
    const div = document.createElement('div');
    div.classList.add('cal-property');

    const p = document.createElement('p');
    p.innerText = `${key} : ${value}`;

    div.appendChild(p); 
    return div;
}