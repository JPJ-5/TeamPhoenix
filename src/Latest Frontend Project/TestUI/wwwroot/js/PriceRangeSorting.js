let currentPage = 1;
let pageSize = document.getElementById('pageSize').value;

function fetchItems() {
    const bottomPrice = document.getElementById('bottomPrice').value;
    const topPrice = document.getElementById('topPrice').value;
    const name = document.getElementById('searchInput').value;
    const loadingIndicator = document.getElementById('loading');
    const results = document.getElementById('results');

    loadingIndicator.style.display = 'block';
    results.innerHTML = '';

    let url = `http://localhost:8080/Item/api/pagedFilteredItems?pageNumber=${currentPage}&pageSize=${pageSize}`;
    if (name) {
        url += `&name=${encodeURIComponent(name)}`;
    }
    if (bottomPrice && topPrice) {
        url += `&bottomPrice=${bottomPrice}&topPrice=${topPrice}`;
    }

    fetch(url)
        .then(response => response.json())
        .then(data => {
            displayResults(data.items);
            const totalPageCount = Math.ceil(data.totalCount / pageSize);
            document.getElementById('pageInfo').textContent = `Page ${currentPage} / ${totalPageCount}`;
            document.getElementById('prevPage').disabled = currentPage <= 1;
            document.getElementById('nextPage').disabled = currentPage >= totalPageCount;
        })
        .catch(error => {
            console.error('Error fetching data:', error);
            results.innerHTML = `<p>Error: ${error.message}</p>`;
        })
        .finally(() => {
            loadingIndicator.style.display = 'none';
        });
}

function searchItems() {
    const nameInput = document.getElementById('searchInput').value.trim();
    const isValid = /^[a-zA-Z\s]+$/.test(nameInput); // Regex to allow only letters and spaces

    if (!nameInput) {
        alert("Please enter a search term.");
        return;
    }
    if (!isValid) {
        alert("Please enter a valid search term using only letters.");
        return;
    }
    currentPage = 1; // Reset to first page for new search results
    fetchItems(); // Perform the fetch with the query
}

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
    fetchItems(); // Apply new filters and reset pagination
}

function displayResults(items) {
    const results = document.getElementById('results');
    const viewFormat = document.getElementById('viewFormat').value;
    results.innerHTML = '';
    if (items.length === 0) {
        results.innerHTML = '<p>No items found.</p>';
        return;
    }

    items.forEach(item => {
        const card = document.createElement('div');
        card.className = viewFormat === 'list' ? 'item-card-list' : 'item-card-grid';
        const content = `
            <div class="item-name">${item.name}</div>
            <div class="item-price">$${item.price.toFixed(2)}</div>
        `;

        if (viewFormat === 'list') {
            // Add your list view HTML structure here
            card.innerHTML = `
                    <img src="images/wallpaperflare.com_wallpaper.jpg" style="width: 225px; height: 218px; object-fit: cover;" class="item-image" />
                    ${content}
            `;
        } else {
            // Your existing grid view HTML structure
            const content = `
            <img src="images/wallpaperflare.com_wallpaper.jpg" style="width: 225px; height: 218px; object-fit: cover;" class="item-image" />
            <div class="item-name">${item.name}</div>
            <div class="item-price">$${item.price.toFixed(2)}</div>`;
            card.innerHTML = content;
        }

        results.appendChild(card);
    });
}

function changePage(direction) {
    const nextPage = direction === 'next' ? currentPage + 1 : currentPage - 1;
    if (nextPage < 1) return; // Prevent going to non-existing negative page numbers

    // Immediately disable both buttons to prevent multiple requests
    document.getElementById('prevPage').disabled = true;
    document.getElementById('nextPage').disabled = true;

    currentPage = nextPage;
    fetchItems(); // Fetch items for the new page
}

function updateViewFormat() {
    fetchItems(); // Reload items to display with the new format
}


function updatePageSize() {
    pageSize = parseInt(document.getElementById('pageSize').value);
    currentPage = 1; // Reset to the first page
    fetchItems(); // Reload with the new page size
}

function initPage() {
    fetchItems(); // Initial fetch for default or saved filter states
}

document.addEventListener('DOMContentLoaded', initPage); // Ensures the script runs after the document is fully loaded