var baseUrl = 'http://localhost:8080';

// Button Event Listeners
document.getElementById('itemModificationBtn').addEventListener('click', function () {
    hideAllSections();
    document.getElementById('itemModificationContainer').style.display = 'block';
    fetchData(1); // Fetch and populate the items
    // Clear input fields in the item modification form
    const form = document.getElementById('modificationForm');
    clearFormFields(form);
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
}

// Fetch Data Functions
function fetchData(page) {
    const username = sessionStorage.getItem('username');
    const url = `${baseUrl}/api/ItemPagination?username=${encodeURIComponent(username)}&pageNumber=${page}&pageSize=10`;

    fetch(url)
        .then(response => response.json())
        .then(data => {
            renderItems(data.items);
            setupPagination('fetchData', data.totalCount, 10, page);
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
            window.location.href = `itemDetail.html?sku=${item.sku}`;
        });

        // Append the row to the table body
        tbody.appendChild(row);
    });
}

document.getElementById('pendingSaleBtn').addEventListener('click', function () {
    hideAllSections();
    document.getElementById('pendingSaleContainer').style.display = 'block';
    fetchDataSalePending(1); // Fetch and populate the items
});

// Setup Pagination Function
function setupPagination(fetchFunction, totalCount, pageSize, currentPage) {
    const totalPages = Math.ceil(totalCount / pageSize);
    const paginationContainer = document.getElementById('pagination');
    if (!paginationContainer) return;
    paginationContainer.innerHTML = ''; // Clear existing pagination

    for (let page = 1; page <= totalPages; page++) {
        const pageButton = document.createElement('button');
        pageButton.textContent = page;
        pageButton.classList.add('page-button');
        pageButton.disabled = (page === currentPage);
        pageButton.onclick = function () {
            if (fetchFunction === 'fetchData') {
                fetchData(page);
            } else if (fetchFunction === 'fetchDataSalePending') {
                fetchDataSalePending(page);
            }
        };
        paginationContainer.appendChild(pageButton);
    }
}

function fetchDataSalePending(page) {
    const username = sessionStorage.getItem('username');
    const url = `${baseUrl}/api/ItemBuying/GetItemPendingSale?username=${encodeURIComponent(username)}&pageNumber=${page}&pageSize=10`;

    fetch(url)
        .then(response => response.json())
        .then(data => {
            renderItemsSalePending(data);
            setupPagination('fetchDataSalePending', data.totalCount, 10, page);
        })
        .catch(error => console.error('Error fetching pending sale data:', error));
}

function renderItemsSalePending(data) {
    const tbody = document.getElementById('saleAcceptTableBody');
    if (!tbody) return;
    tbody.innerHTML = ''; // Clear previous items

    if (!data.receipts) {
        console.error('No receipts found in data:', data);
        return;
    }

    // Loop through each receipt and display it
    data.receipts.forEach(receipt => {
        const item = receipt.item;
        if (!item) {
            console.error('No item found for receipt:', receipt);
            return;
        }

        const imageUrl = item.firstImage || 'css/default_image.jpg';

        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${item.name}</td>
            <td>${item.sku}</td>
            <td><img src="${imageUrl}" alt="Item Image" style="width: 100px;"></td>
            <td>$${item.price.toFixed(2)}</td>
            <td>${item.stockAvailable}</td>
            <td>${receipt.quantity}</td>
            <td>$${receipt.offerPrice.toFixed(2)}</td>
            <td>$${receipt.revenue.toFixed(2)}</td>
            <td>$${receipt.profit.toFixed(2)}</td>
            <td>${new Date(receipt.saleDate).toLocaleDateString()}</td>
        `;

        // Create the actions cell (Accept and Decline buttons)
        const actionsCell = document.createElement('td');

        // Accept button
        const acceptButton = document.createElement('button');
        acceptButton.textContent = 'Accept';
        acceptButton.classList.add('btn', 'accept');
        acceptButton.onclick = function (event) {
            event.stopPropagation(); // Prevent the row click event from being triggered
            acceptItemSale(receipt.receiptID, receipt.sku, receipt.quantity);
        };
        actionsCell.appendChild(acceptButton);

        // Decline button
        const declineButton = document.createElement('button');
        declineButton.textContent = 'Decline';
        declineButton.classList.add('btn', 'decline');
        declineButton.onclick = function (event) {
            event.stopPropagation(); // Prevent the row click event from being triggered
            if (confirm(`Are you sure you want to decline the sale of receipt ID: ${receipt.receiptID}?`)) {
                declineItemSale(receipt.receiptID, receipt.sku, receipt.quantity);
            }
        };
        actionsCell.appendChild(declineButton);

        // Append the actions cell to the row
        row.appendChild(actionsCell);

        // Append the row directly to the table body
        tbody.appendChild(row);
    });
}

// Accept Item Sale
function acceptItemSale(receiptID, sku, quantity) {
    const username = sessionStorage.getItem('username');
    const idToken = sessionStorage.getItem('idToken');
    const accessToken = sessionStorage.getItem('accessToken');
    const request = {
        ReceiptID: receiptID,
        SKU: sku,
        Quantity: quantity
    };

    fetch(`${baseUrl}/api/ItemBuying/AcceptPendingSale`, {
        method: 'POST',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'userName': username,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok ' + response.statusText);
            }
            return response.json();
        })
        .then(data => {
            alert('You accepted the sale. Confirmation Emails are sent to both buyer and seller!');
            fetchDataSalePending(1); // Refresh the list of pending sales
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Failed to accept the sale.');
        });
}

// Decline Item Sale
function declineItemSale(receiptID, sku, quantity) {
    const username = sessionStorage.getItem('username');
    const idToken = sessionStorage.getItem('idToken');
    const accessToken = sessionStorage.getItem('accessToken');
    const request = {
        ReceiptID: receiptID,
        SKU: sku,
        Quantity: quantity
    };

    fetch(`${baseUrl}/api/ItemBuying/DeclinePendingSale`, {
        method: 'DELETE',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'userName': username,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok ' + response.statusText);
            }
            return response.json();
        })
        .then(data => {
            alert('You declined the sale. The receipt is deleted!');
            fetchDataSalePending(1); // Refresh the list of pending sales
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Failed to decline the sale.');
        });
}

// Modify Item
function modifyItem(sku) {
    hideAllSections();
    document.getElementById('itemModificationForm').style.display = 'block';
    fetchItemDetailsModification(sku);
}

// Fetch Item Details
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

    // Remove any existing event listeners
    form.removeEventListener('submit', handleSubmit);

    // Add new event listener
    form.addEventListener('submit', handleSubmit);

    async function handleSubmit(event) {
        event.preventDefault(); // Prevent default form submission

        const formData = new FormData(form);
        const jsonData = Object.fromEntries(formData.entries());

        // Ensure checkboxes are correctly included
        jsonData.offerablePrice = form.offerablePrice.checked;
        jsonData.itemListed = form.itemListed.checked;
        jsonData.sku = sku;

        // Handle file uploads separately
        jsonData.imageUrls = [...form.image.files].map(file => file.name);
        jsonData.videoUrls = [...form.video.files].map(file => file.name);

        await uploadFilesToSandbox(form)

        await updateItem(jsonData, form);
        //add another function here to update item
    }
}

// Function to update the item in the backend
//async function updateItem(item, form) {
//    const username = sessionStorage.getItem('username');
//    const idToken = sessionStorage.getItem("idToken");
//    const accessToken = sessionStorage.getItem("accessToken");

//    // FormData to hold the file data for the first call
//    var formData = new FormData(form);
//    var images = document.querySelector('[name="image[]"]').files;
//    Array.from(images).forEach(file => {
//        formData.append('files', file);
//    });
//    var videos = document.querySelector('[name="video[]"]').files;
//    Array.from(videos).forEach(video => {
//        formData.append('files', video);
//    });

//    try {
//        if (formData != null) { // if user has new pic or video, upload it to backend sandbox, if not, skip the upload.
//            // First Fetch Call to upload files
//            var uploadPath = `${baseUrl}/api/UploadS3/UploadFilesToSandBox`;

//            let uploadResponse = await fetch(uploadPath, {
//                method: 'POST',
//                headers: {
//                    'Authentication': idToken,
//                    'Authorization': accessToken,
//                    'userName': username
//                },
//                body: formData
//            });
//            if (!uploadResponse.ok) {
//                alert('There was an error while uploading your items pictures and videos. Please try again!');
//                let text = await uploadResponse.text();
//                throw new Error(`Error uploading files to sandbox: ${text}`);
//            }

//            // Assuming the server responds with JSON data
//            let uploadData = await uploadResponse.json();
//            let images = "string";
//            let videos = "string";

//            // Second Fetch Call to modify item
//            var itemCreatePath = `${baseUrl}/api/ItemModification/UpdateItem`;

//            const sku = item.sku;
//            const name = formData.get('name');
//            const price = formData.get('price');
//            const description = formData.get('description');
//            const stockAvailable = formData.get('stockAvailable');
//            const productionCost = formData.get('productionCost');
//            const offerablePrice = Boolean(formData.get('offerablePrice'));
//            const sellerContact = formData.get('sellerContact');
//            const listed = Boolean(formData.get('itemListed'));

//            const data = { name, sku, price, description, stockAvailable, productionCost, offerablePrice, sellerContact, images, videos, listed };

//            let itemCreationResponse = await fetch(itemCreatePath, {
//                method: 'POST',
//                headers: {
//                    'Authentication': idToken,
//                    'Authorization': accessToken,
//                    'userName': username,
//                    'Content-Type': 'application/json'
//                },
//                body: JSON.stringify(data)
//            });

//            if (!itemCreationResponse.ok) {
//                alert('Error occurred while adding item to database. Please try again!');
//                let text = await itemCreationResponse.text();
//                throw new Error(`Error during item creation: ${text}`);
//            } else {
//                alert('Your item is successfully added to CraftVerify!');
//            }
//        }
//    } catch (error) {
//        console.error('Error:', error);
//    }
//}

// Function to upload files to the backend sandbox
async function uploadFilesToSandbox(formData) {
    const username = sessionStorage.getItem('username');
    const idToken = sessionStorage.getItem("idToken");
    const accessToken = sessionStorage.getItem("accessToken");


    var images = document.querySelector('[name="image[]"]').files;
    Array.from(images).forEach(file => {
        formData.append('files', file);
    });
    var videos = document.querySelector('[name="video[]"]').files;
    Array.from(videos).forEach(video => {
        formData.append('files', video);
    });

    //const uploadPath = `${baseUrl}/api/UploadS3/UploadFilesToSandBox`;

    let uploadResponse = await fetch(`${baseUrl}/api/UploadS3/UploadFilesToSandBox`, {
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
        throw new Error(`Error uploading files to sandbox: ${text}`);
    }

    // Assuming the server responds with JSON data
    let uploadData = await uploadResponse.json();
    return uploadData;
}

// Function to update the item in the backend
async function updateItem(item, form) {
    const username = sessionStorage.getItem('username');
    const idToken = sessionStorage.getItem("idToken");
    const accessToken = sessionStorage.getItem("accessToken");

    var formData = new FormData(form);

    const sku = item.sku;
    const name = formData.get('name');
    const price = formData.get('price');
    const description = formData.get('description');
    const stockAvailable = formData.get('stockAvailable');
    const productionCost = formData.get('productionCost');
    const offerablePrice = Boolean(formData.get('offerablePrice'));
    const sellerContact = formData.get('sellerContact');
    const listed = Boolean(formData.get('itemListed'));

    const data = { name, sku, price, description, stockAvailable, productionCost, offerablePrice, sellerContact, images, videos, listed };


    const itemCreatePath = `${baseUrl}/api/ItemModification/UpdateItem`;

    let itemCreationResponse = await fetch(itemCreatePath, {
        method: 'POST',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'userName': username,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    });

    if (!itemCreationResponse.ok) {
        alert('Error occurred while adding item to database. Please try again!');
        let text = await itemCreationResponse.text();
        throw new Error(`Error during item creation: ${text}`);
    } else {
        alert('Your item is successfully added to CraftVerify!');
    }
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
        jsonData.itemListed = newForm.itemListed.checked;
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
        throw new Error(`Error uploading files to sandbox: ${text} `);
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

    let itemCreationResponse = await fetch(itemCreatePath, {
        method: 'POST',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'userName': username,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(item)
    });

    if (!itemCreationResponse.ok) {
        alert('Error occurred while adding item to database. Please try again!');
        let text = await itemCreationResponse.text();
        throw new Error(`Error during item creation: ${text}`);
    } else {
        alert('Your item is successfully added to CraftVerify!');
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
            fetchData(1); // Refresh the list
        })
        .catch(error => {
            console.error('Error deleting item:', error);
            alert('Failed to delete item.');
        });
}
