var baseUrl = 'https://themusicali.com:5000';
//var baseUrl = 'http://localhost:8080';




// Button Event Listeners
document.getElementById('itemModificationBtn').addEventListener('click', function () {
    hideAllSections();
    const idToken = sessionStorage.getItem('idToken');
    const accessToken = sessionStorage.getItem('accessToken');
    const username = sessionStorage.getItem('username');

    if (!idToken || !accessToken || !username) {
        alert("Please login to use this feature!!!");
    }
    else {
        document.getElementById('itemModificationContainer').style.display = 'block';
        fetchModificationData(1); // Fetch and populate the items
        // Clear input fields in the item modification form
        const form = document.getElementById('modificationForm');
        clearFormFields(form);
    }
});

function clearFormFields(form) {
    form.reset(); // Resets all input values to their defaults
    form.querySelectorAll('input[type="checkbox"]').forEach((checkbox) => {
        checkbox.checked = false; // Uncheck all checkboxes
    });
    form.querySelectorAll('input[type="file"]').forEach((fileInput) => {
        fileInput.value = ''; // Clear file inputs
    });
}

function hideAllSections() {
    document.getElementById('itemCreationForm').style.display = 'none';
    document.getElementById('itemModificationContainer').style.display = 'none';
    document.getElementById('itemModificationForm').style.display = 'none';
    document.getElementById('pendingSaleContainer').style.display = 'none';
    document.getElementById('financialProgressReportView').style.display = 'none';
    document.getElementById('itemsListingContainer').style.display = 'none';
    document.getElementById('priceRangeSortingView').style.display = 'none';
    if (document.getElementById('itemDetailsContainer')) {
        document.getElementById('itemDetailsContainer').style.display = 'none';
    }
}

// Fetch Data Functions
function fetchModificationData(page) {
    const username = sessionStorage.getItem('username');
    const url = `${baseUrl}/api/ItemPagination?username=${encodeURIComponent(username)}&pageNumber=${page}&pageSize=10`;

    fetch(url)
        .then(response => response.json())
        .then(data => {
            renderItems(data.items);
            setupPaginationModification('fetchModificationData', data.totalCount, 10, page);
        })
        .catch(error => console.error('Error fetching data:', error));
}

// Render Functions
function renderItems(items) {
    const tbody = document.getElementById('itemTableBody');
    if (!tbody) return;
    tbody.innerHTML = ''; // Clear previous items

    items.forEach(item => {
        const imageUrl = item.firstImage || 'css/default_image.jpg';

        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${item.name}</td>
            <td>$${item.price.toFixed(2)}</td>
            <td><img src="${imageUrl}" alt="Item Image" style="width: 100px;"></td>
            <td>${item.stockAvailable}</td>
        `;

        // Create the actions cell (Modify and Delete buttons)
        const actionsCell = document.createElement('td');

        // Modify button
        const modifyButton = document.createElement('button');
        modifyButton.textContent = 'Modify';
        modifyButton.classList.add('btn', 'modify');
        modifyButton.onclick = function (event) {
            event.stopPropagation(); // Prevent the row click event from being triggered
            modifyItem(item.sku);
        };
        actionsCell.appendChild(modifyButton);

        // Delete button
        const deleteButton = document.createElement('button');
        deleteButton.textContent = 'Delete';
        deleteButton.classList.add('btn', 'delete');
        deleteButton.onclick = function (event) {
            event.stopPropagation(); // Prevent the row click event from being triggered
            if (confirm(`Are you sure you want to delete the item: ${item.name}?`)) {
                deleteItem(item.sku);
            }
        };
        actionsCell.appendChild(deleteButton);

        // Append the actions cell to the row
        row.appendChild(actionsCell);

        // Add a row click event to navigate to the item detail page
        row.addEventListener('click', () => {
            document.getElementById('itemModificationContainer').style.display = 'none';
            // Item Detail
            const container = document.getElementById('itemDetailView');
            container.style.display = 'block';

            // Load the CSS dynamically
            const cssLink = document.createElement('link');
            cssLink.rel = 'stylesheet';
            cssLink.href = 'css/ItemDetail.css';
            document.head.appendChild(cssLink);

            fetch('ItemDetail.html')
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Failed to load Item Detail HTML.');
                    }
                    return response.text();
                })
                .then(html => {
                    container.innerHTML = html;

                    // Initialize JavaScript functionalities after HTML is loaded
                    const jsScript = document.createElement('script');
                    jsScript.src = 'ItemCreationListingBuyingFeature/ItemDetail.js'; // Ensure this path is correct
                    jsScript.onload = function () {
                        loadDetail(item.sku,2);
                        // JavaScript file loaded and executed
                    };
                    jsScript.onerror = function () {
                        console.error('Failed to load Item Detail JS.');
                    };
                    document.body.appendChild(jsScript);  // Append and execute after HTML content is loaded
                })
                .catch(error => {
                    console.error('Error loading Item Detail View:', error);
                });
        });

        // Append the row to the table body
        tbody.appendChild(row);
    });
}



// Setup Pagination Function
function setupPaginationModification(fetchFunction, totalCount, pageSize, currentPage) {
    console.log(totalCount, pageSize);
    const totalPages = Math.ceil(totalCount / pageSize);
    const paginationContainer = document.getElementById('paginationModification');
    if (!paginationContainer) return;
    paginationContainer.innerHTML = '';

    for (let page = 1; page <= totalPages; page++) {
        const pageButton = document.createElement('button');
        pageButton.textContent = page;
        pageButton.classList.add('page-button');
        pageButton.disabled = (page === currentPage);
        pageButton.setAttribute('aria-label', `Go to page ${page}`);
        pageButton.onclick = () => window[fetchFunction](page);
        paginationContainer.appendChild(pageButton);
    }
}



// Modify Item
function modifyItem(sku) {
    hideAllSections();
    document.getElementById('itemModificationForm').style.display = 'block';
    fetchItemDetailsModification(sku);
}

// Fetch Item Details to setup form modification
function fetchItemDetailsModification(sku) {
    const url = `${baseUrl}/api/GetItemDetail?sku=${sku}`;

    fetch(url, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok ' + response.statusText);
            }
            return response.json();
        })
        .then(item => {
            populateFormFields(item);   // Populate form fields with the item details
            setupFormHandlers(sku);     // Set up form handlers
        })
        .catch(error => {
            console.error('Error fetching item details:', error);
        });
}

// Populate Form Fields
function populateFormFields(item) {
    document.getElementById('name').value = item.name;
    document.getElementById('price').value = item.price;
    document.getElementById('description').value = item.description;
    document.getElementById('stockAvailable').value = item.stockAvailable;
    document.getElementById('productionCost').value = item.productionCost;
    document.getElementById('offerablePrice').checked = item.offerablePrice;
    document.getElementById('itemListed').checked = item.listed;
    document.getElementById('sellerContact').value = item.sellerContact;
}

// Function to set up form handlers
function setupFormHandlers(sku) {
    const form = document.getElementById('modificationForm');
    // Remove existing event listeners to prevent multiple instances
    const newForm = form.cloneNode(true);
    form.parentNode.replaceChild(newForm, form);

    newForm.addEventListener('submit', async function (event) {
        event.preventDefault(); // Prevent default form submission

        const formData = new FormData(newForm);
        const jsonData = Object.fromEntries(formData.entries());

        // Ensure checkboxes are correctly included
        jsonData.offerablePrice = newForm.offerablePrice.checked;
        jsonData.listed = newForm.itemListed.checked;
        jsonData.sku = sku;

        // Handle file uploads separately
        jsonData.imageUrls = [...newForm.image.files].map(file => file.name);
        jsonData.videoUrls = [...newForm.video.files].map(file => file.name);

        // First, upload files to sandbox
        await uploadFilesToSandbox(formData);

        // Then, call updateItem function
        await updateItem(jsonData);
    });
}

// Function to upload files to the backend sandbox
async function uploadFilesToSandbox(formData) {
    const username = sessionStorage.getItem('username');
    const idToken = sessionStorage.getItem("idToken");
    const accessToken = sessionStorage.getItem("accessToken");

    const uploadPath = `${baseUrl}/api/UploadS3/UploadFilesToSandBox`;

    let uploadResponse = await fetch(uploadPath, {
        method: 'POST',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'userName': username
        },
        body: formData
    });

    if (!uploadResponse.ok) {
        alert('There was an error while uploading your items pictures and videos. Please try again!');
        let text = await uploadResponse.text();
        throw new Error(`Error uploading files to sandbox`);
    }

    // Assuming the server responds with JSON data
    let uploadData = await uploadResponse.json();
    return uploadData;
}

// Function to update the item in the backend
async function updateItem(item) {
    const username = sessionStorage.getItem('username');
    const idToken = sessionStorage.getItem("idToken");
    const accessToken = sessionStorage.getItem("accessToken");

    const itemCreatePath = `${baseUrl}/api/ItemModification/UpdateItem`;

    let itemModificationResponse = await fetch(itemCreatePath, {
        method: 'POST',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'userName': username,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(item)
    });

    if (!itemModificationResponse.ok) {
        alert('Error occurred while modifying item. Please try again!');
        let text = await itemModificationResponse.text();
        throw new Error(`Error during item creation: ${text}`);
    } else {
        alert('Your item is successfully modified to CraftVerify!');
    }
}

function deleteItem(sku) {
    const url = `${baseUrl}/api/ItemDeletion/DeleteItem`;
    const username = sessionStorage.getItem('username');
    const idToken = sessionStorage.getItem('idToken');
    const accessToken = sessionStorage.getItem('accessToken');

    fetch(url, {
        method: 'DELETE',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'userName': username,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(sku)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to delete item: ' + response.statusText);
            }
            alert('Item deleted successfully!');
            fetchModificationData(1); // Refresh the list
        })
        .catch(error => {
            console.error('Error deleting item:', error);
            alert('Failed to delete item.');
        });
}
