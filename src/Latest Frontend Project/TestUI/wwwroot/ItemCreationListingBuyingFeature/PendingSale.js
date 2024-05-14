var baseUrl = 'https://themusicali.com:5000';
//var baseUrl = 'http://localhost:8080';

// set up pending sale button to call list of pending sale
document.getElementById('pendingSaleBtn').addEventListener('click', function () {
    hideAllSectionsFromCraft();
    const idToken = sessionStorage.getItem('idToken');
    const accessToken = sessionStorage.getItem('accessToken');
    const username = sessionStorage.getItem('username');
    if (!idToken || !accessToken || !username) {
        alert("Please login to use this feature!!!");
    }
    else {
        document.getElementById('pendingSaleContainer').style.display = 'block';

        fetchDataSalePending(1); // Fetch and populate the items
    }
   
});

//fetch call pending sale
function fetchDataSalePending(page) {
    const username = sessionStorage.getItem('username');
    const url = `${baseUrl}/api/ItemBuying/GetItemPendingSale?username=${encodeURIComponent(username)}&pageNumber=${page}&pageSize=10`;

    fetch(url)
        .then(response => response.json())
        .then(data => {
            renderItemsSalePending(data);
            setupPaginationPendingSale('fetchDataSalePending', data.totalCount, 10, page);
        })
        .catch(error => console.error('Error fetching pending sale data:', error));
}

function setupPaginationPendingSale(fetchFunction, totalCount, pageSize, currentPage) {
    console.log(totalCount, pageSize);
    const totalPages = Math.ceil(totalCount / pageSize);
    const paginationContainer = document.getElementById('paginationPendingSale');
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