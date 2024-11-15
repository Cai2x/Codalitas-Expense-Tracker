// wwwroot/js/expense-modals.js

function fetchCategories() {
    fetch('/Expense/Category')
        .then(response => response.json())
        .then(data => {
            // Populate dropdowns with categories
            const categorySelects = document.querySelectorAll('.category-dropdown');
            categorySelects.forEach(select => {
                select.innerHTML = ''; // Clear current options
                data.forEach(category => {
                    const option = document.createElement('option');
                    option.value = category.id;
                    option.text = category.name;
                    select.appendChild(option);
                });
            });
        })
        .catch(error => console.error('Error fetching categories:', error));
}
