
function setupInventoryStockView() {
    fetchInventoryStock();
}

var baseUrl = 'http://localhost:8080';

function fetchInventoryStock() {
    const username = sessionStorage.getItem("username");
    const resultsDiv = document.getElementById('inventoryResults');
    const loadingIndicator = document.getElementById('loading');
    idToken = sessionStorage.getItem("idToken");
    accessToken = sessionStorage.getItem("accessToken");

    loadingIndicator.style.display = 'block';
    resultsDiv.innerHTML = '';

    fetch(`${baseUrl}/SellerDashboard/api/GetInventoryStock`, {
        method: 'GET',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'userName': username
        }
    })
        .then(response => {
            if (!response.ok) throw new Error('Failed to fetch inventory data: ' + response.statusText);
            return response.json();
        })
        .then(data => {
            if (data.length === 0) {
                resultsDiv.innerHTML = '<p>No inventory items found for the user.</p>';
            } else {
                const list = document.createElement('ul');
                data.forEach(item => {
                    const listItem = document.createElement('li');
                    listItem.textContent = `${item.name} (SKU: ${item.sku}) - ${item.stockAvailable} available - $${item.price}`;
                    list.appendChild(listItem);
                });
                resultsDiv.appendChild(list);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            resultsDiv.innerHTML = `<p>${error.message}</p>`;
        })
        .finally(() => {
            loadingIndicator.style.display = 'none';
        });
}