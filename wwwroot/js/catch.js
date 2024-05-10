function validateRequiredFields() {
    const form = document.querySelector('form');
    form.addEventListener('submit', function(event) {
        const requiredFields = form.querySelectorAll('[required]');

        requiredFields.forEach(function(field) {
            if (!field.value.trim()) {
                field.classList.add('required-field'); 
                event.preventDefault(); 
                alert(`${field.name} cannot be null.`); 
            } else {
                field.classList.remove('required-field'); 
            }
        });
    });
    form.addEventListener('input', function(event) {
        const target = event.target;
        if (target.hasAttribute('required')) {
            if (target.value.trim()) {
                target.classList.remove('required-field');
            } else {
                target.classList.add('required-field');
            }
        }
    });
}

document.addEventListener('DOMContentLoaded', function() {
    validateRequiredFields();
});
