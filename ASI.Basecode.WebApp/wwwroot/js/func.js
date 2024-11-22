function fetchCategories(dropdownElement, id) {
    fetch('/Expense/Category')
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const categories = data.data;

                // Clear existing options
                dropdownElement.innerHTML = '';

                // Default "Select Category" option
                if (id == null && dropdownElement.id !== "filterCategory") {
                    const defaultOption = document.createElement('option');
                    defaultOption.value = '';
                    defaultOption.textContent = 'Select Category';
                    defaultOption.hidden = true;
                    dropdownElement.appendChild(defaultOption);
                }

                if (dropdownElement.id === "filterCategory") {
                    const defaultOption = document.createElement('option');
                    defaultOption.value = '';
                    defaultOption.textContent = 'All';
                    dropdownElement.appendChild(defaultOption);
                }
                // Populate the dropdown with categories
                categories.forEach(category => {
                    const option = document.createElement('option');
                    option.value = category.categoryId;
                    option.textContent = category.name;
                    dropdownElement.appendChild(option);

                    if (id != null && category.categoryId === id) {
                        option.selected = true;
                    }
                });
            } else {
                const defaultOption = document.createElement('option');
                defaultOption.value = '';
                defaultOption.textContent = 'No Categories Added';
                defaultOption.hidden = true;
                dropdownElement.appendChild(defaultOption);
            }
        })
        .catch(error => {
            console.error('Error fetching categories:', error);
        });
}

var loadURL = '@Url.Action("SearchFilter", "Expense")' 
// Load data with pagination and sorting
function loadData(page, expenseName, sortOption, category) {
    $.ajax({
        url: '/Expense/SearchFilter',
        type: 'GET',
        data: { page: page, expenseName: expenseName, sortType: sortOption, filterCategory: category, pageSize: 7 },
        success: function (response) {
            updateTable(response.data);  // Update the table with the sorted data
            updatePagination(response.totalPages, response.currentPage);  // Update pagination controls
        },
        error: function () {
            alert('An error occurred while fetching sorted data');
        }
    });
}

// Update the table with the data from the server
function updateTable(data) {
    var tbody = document.querySelector('table tbody');
    tbody.innerHTML = '';  // Clear the current table content

    data.forEach(function (item) {
        var row = document.createElement('tr');
        var formattedAmount = parseFloat(item.amount).toLocaleString('en-US', { style: 'currency', currency: 'USD' });

        // Add columns to the row
        row.innerHTML = `
                        <td>${item.title}</td>
                        <td><span class="badge bg-secondary">${item.categoryName}</span></td>
                        <td>${formattedAmount}</td>
                        <td>
                            ${item.status ? '<span class="badge bg-success">Published</span>' : '<span class="badge bg-warning">Draft</span>'}
                        </td>
                        <td>
                            <a href="javascript:void(0);" onclick="openEditModal(${item.expenseId})" class="btn btn-warning" data-bs-toggle="modal" data-bs-target="#editExpenseModal">
                                <i class="fas fa-pencil-alt"></i>
                            </a>
                            <a href="javascript:void(0);" onclick="openDeleteModal(${item.expenseId})" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#deleteExpenseModal">
                                <i class="fas fa-trash"></i>
                            </a>
                        </td>
                    `;
        tbody.appendChild(row);  // Append the row to the table body
    });
}

// Update the pagination controls
function updatePagination(totalPages, currentPage) {
    var pagination = document.querySelector('.pagination');
    pagination.innerHTML = '';  // Clear existing pagination controls

    // Add "Previous" button if necessary
    if (currentPage > 1) {
        pagination.innerHTML += `<li class="page-item">
                        <a class="page-link" href="javascript:void(0);" onclick="changePage(${currentPage - 1})">Previous</a>
                    </li>`;
    }

    // Add page number buttons
    for (var i = 1; i <= totalPages; i++) {
        pagination.innerHTML += `<li class="page-item ${i === currentPage ? 'active' : ''}">
                        <a class="page-link" href="javascript:void(0);" onclick="changePage(${i})">${i}</a>
                    </li>`;
    }

    // Add "Next" button if necessary
    if (currentPage < totalPages) {
        pagination.innerHTML += `<li class="page-item">
                        <a class="page-link" href="javascript:void(0);" onclick="changePage(${currentPage + 1})">Next</a>
                    </li>`;
    }
}

// Change the current page and reload the data
function changePage(page) {
    currentPage = page;  // Update current page
    loadData(currentPage, currentSearch, currentSort, currentCategory);  // Reload the data with new page and current sorting
}