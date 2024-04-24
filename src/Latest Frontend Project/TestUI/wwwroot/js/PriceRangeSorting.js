function fetchItems() {
    const bottomPrice = document.getElementById('bottomPrice').value;
    const topPrice = document.getElementById('topPrice').value;
    const loadingIndicator = document.getElementById('loading');
    const results = document.getElementById('results');

    if (parseInt(topPrice) < parseInt(bottomPrice)) {
        alert("Maximum price should be greater than minimum price.");
        return;
    }

    loadingIndicator.style.display = 'block';
    results.innerHTML = '';

    fetch(`http://localhost:8080/Item/api/sort?bottomPrice=${bottomPrice}&topPrice=${topPrice}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch items: ' + response.statusText);
            }
            return response.json();
        })
        .then(data => displayResults(data.data))
        .catch(error => {
            console.error('Error fetching data:', error);
            results.innerHTML = `<p>Error: ${error.message}</p>`;
        })
        .finally(() => {
            loadingIndicator.style.display = 'none';
        });
}

function searchItems() {
    const query = document.getElementById('searchInput').value;
    if (!query) {
        alert("Please enter a search term.");
        return;
    }

    fetch(`http://localhost:8080/Item/api/search?query=${encodeURIComponent(query)}`)
        .then(response => {
            if (!response.ok) {
                if (response.status === 404) {
                    throw new Error('No items found matching your search.');
                }
                throw new Error('Network response was not ok: ' + response.statusText);
            }
            return response.json();
        })
        .then(data =>  displayResults(data.data))
        .catch(error => {
            console.error('Error fetching data:', error);
            alert(error.message);
        });
}

let currentPage = 1;
let pageSize = document.getElementById('pageSize').value;

document.getElementById('pageSize').value = pageSize;

function updatePageSize() {
    pageSize = parseInt(document.getElementById('pageSize').value);
    loadPageItems(currentPage); // Reload with the new page size
}

function loadPageItems(page, bottomPrice = null, topPrice = null) {
    const pageInfo = document.getElementById('pageInfo');
    const loadingIndicator = document.getElementById('loading');
    const results = document.getElementById('results');
    currentPage = page;

    let url = `http://localhost:8080/Item/api/pagedItems?pageNumber=${currentPage}&pageSize=${pageSize}`;
    if (bottomPrice && topPrice) {
        url += `&bottomPrice=${bottomPrice}&topPrice=${topPrice}`;
    }

    loadingIndicator.style.display = 'block';
    results.innerHTML = '';

    fetch(url)
        .then(response => response.json())
        .then(data => {
            if (!data || !data.items || data.items.length === 0) {
                results.innerHTML = '<p>No items found.</p>';
                pageInfo.textContent = `Page ${currentPage} of 0`;
            } else {
                displayResults(data.items);
                const totalPageCount = Math.ceil(data.totalCount / pageSize);
                pageInfo.textContent = `Page ${currentPage} of ${totalPageCount}`;
                document.getElementById('prevPage').disabled = currentPage <= 1;
                document.getElementById('nextPage').disabled = currentPage >= totalPageCount;
            }
        })
        .catch(error => {
            console.error('Error fetching data:', error);
            results.innerHTML = `<p>Error: ${error.message}</p>`;
        })
        .finally(() => {
            loadingIndicator.style.display = 'none';
        });
}

// Use this function to initialize or update page data
document.addEventListener('DOMContentLoaded', function () {
    loadPageItems(1); // Load initial page
});

// Pagination controls
function changePage(direction) {
    if (direction === 'next') {
        loadPageItems(currentPage + 1);
    } else if (direction === 'prev') {
        loadPageItems(currentPage - 1);
    }
}

function displayResults(items) {
    const results = document.getElementById('results');
    results.innerHTML = '';
    if (items.length === 0) {
        results.innerHTML = '<p>No items found.</p>';
        return;
    }
    const ul = document.createElement('div');
    items.forEach(item => {
        const card = document.createElement('div');
        card.className = 'item-card';
        card.innerHTML = `
            <div class="item-name">${item.name}</div>
            <div class="item-price">$${item.price.toFixed(2)}</div>
        `;
        results.appendChild(card);
    });
}

// Add the loadPageItems call to setPredefinedRanges to handle price range selection with pagination
function setPredefinedRanges() {
    const range = document.getElementById('predefinedRanges').value;
    if (range) {
        const [min, max] = range.split('-');
        document.getElementById('bottomPrice').value = min;
        document.getElementById('topPrice').value = max;
    } else {
        document.getElementById('bottomPrice').value = '';
        document.getElementById('topPrice').value = '';
    }
    fetchItems();
}

// Event listeners to reset the dropdown when manual price input is modified
document.getElementById('bottomPrice').addEventListener('input', function () {
    document.getElementById('predefinedRanges').value = ""; // Reset dropdown when manual change is made
});

document.getElementById('topPrice').addEventListener('input', function () {
    document.getElementById('predefinedRanges').value = ""; // Reset dropdown when manual change is made
});