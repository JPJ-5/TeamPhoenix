function setupPageComponents() {
    fetchItems(); // Initial fetch for default or saved filter states

    // Add event listeners to reload items when inputs are cleared
    document.getElementById('bottomPrice').addEventListener('input', handleInputClear);
    document.getElementById('topPrice').addEventListener('input', handleInputClear);
    document.getElementById('searchInput').addEventListener('input', handleInputClear);
}

(function () {
    // Track previously used bottom and top prices
    let lastBottomPrice = null;
    let lastTopPrice = null;
    let currentPage = 1;
    let pageSize = document.getElementById('pageSize').value;
    var baseUrl = 'https://themusicali.com:5000';
    //var baseUrl = 'http://localhost:8080';

    // Global variable to store fetched items
    let fetchedItems = [];

    window.fetchItems = function() {
        const bottomPrice = document.getElementById('bottomPrice').value;
        const topPrice = document.getElementById('topPrice').value;
        const name = document.getElementById('searchInput').value;
        const loadingIndicator = document.getElementById('loading');
        const results = document.getElementById('results');

        // Reset page number if the price range changes
        if (bottomPrice !== lastBottomPrice || topPrice !== lastTopPrice) {
            currentPage = 1;
        }

        // Store the last used prices
        lastBottomPrice = bottomPrice;
        lastTopPrice = topPrice;

        loadingIndicator.style.display = 'block';
        results.innerHTML = '';

        // Input validation for prices
        if ((bottomPrice && isNaN(parseFloat(bottomPrice))) || (topPrice && isNaN(parseFloat(topPrice)))) {
            results.innerHTML = '<p>Please enter a valid number for price values.</p>';
            loadingIndicator.style.display = 'none';
            return;
        }

        if (bottomPrice < 0) {
            results.innerHTML = '<p>Please enter a positive value for the bottom price.</p>';
            loadingIndicator.style.display = 'none';
            return;
        }

        if (topPrice > 1000000) {
            results.innerHTML = '<p>The top price should be less than or equal to 1 million.</p>';
            loadingIndicator.style.display = 'none';
            return;
        }

        // Construct the full URL by appending the endpoint to the base URL
        let url = `${baseUrl}/Item/api/pagedFilteredItems?pageNumber=${currentPage}&pageSize=${pageSize}`;
        if (name) {
            url += `&name=${encodeURIComponent(name)}`;
        }
        if (bottomPrice) {
            url += `&bottomPrice=${bottomPrice}`;
        }
        if (topPrice) {
            url += `&topPrice=${topPrice}`;
        }

        fetch(url)
            .then(response => response.json())
            .then(data => {
                fetchedItems = data.data.items; // Store the fetched items
                displayResults(fetchedItems);
                let totalPageCount = Math.ceil(data.data.totalCount / pageSize);

                // Set totalPageCount to 1 if there are no items found
                if (totalPageCount === 0) {
                    totalPageCount = 1;
                }

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

    window.searchItems = function() {
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

    window.setPredefinedRanges = function() {
        const range = document.getElementById('predefinedRanges').value;
        if (range) {
            const [min, max] = range.split('-');
            document.getElementById('bottomPrice').value = min;
            document.getElementById('topPrice').value = max;
        } else {
            document.getElementById('bottomPrice').value = '';
            document.getElementById('topPrice').value = '';
        }

        currentPage = 1;
        fetchItems(); // Apply new filters and reset pagination
    }

    window.displayResults = function(items) {
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

            const imageUrl = item.firstImageUrl || 'images/default.png'; // Use a default image if no URL is provided
            const content = `
                <img src="${imageUrl}" alt="${item.name}" style="width: 225px; height: 218px; object-fit: cover;" class="item-image" />
                <div class="item-name">${item.name}</div>
                <div class="item-price">$${item.price.toFixed(2)}</div>
            `;

            card.innerHTML = content;
            results.appendChild(card);
        });
    }

    window.changePage = function(direction) {
        const nextPage = direction === 'next' ? currentPage + 1 : currentPage - 1;
        if (nextPage < 1) return; // Prevent going to non-existing negative page numbers

        // Immediately disable both buttons to prevent multiple requests
        document.getElementById('prevPage').disabled = true;
        document.getElementById('nextPage').disabled = true;

        currentPage = nextPage;
        fetchItems(); // Fetch items for the new page
    }

    window.updatePageSize = function() {
        pageSize = parseInt(document.getElementById('pageSize').value);
        currentPage = 1; // Reset to the first page
        fetchItems(); // Reload with the new page size
    }

    window.updateViewFormat = function() {
        const results = document.getElementById('results');
        const viewFormat = document.getElementById('viewFormat').value;

        // Remove existing view classes
        results.classList.remove('item-card-list', 'item-card-grid');

        // Add the new view class based on the selected format
        if (viewFormat === 'list') {
            results.classList.add('item-card-list');
        } else {
            results.classList.add('item-card-grid');
        }
        currentPage = 1;
        fetchItems(); // Reload items to display with the new format
    }

    window.updateSortMethod = function() {
        currentSortMethod = document.getElementById('sortMethod').value;
        sortItems(); // Sort items based on the new sort method
        displayResults(fetchedItems); // Re-render items
    }

    window.handleInputClear = function(event) {
        if (event.target.value === '') {
            currentPage = 1;
            fetchItems(); // Reload items when inputs are cleared
        }
    }

    document.addEventListener('DOMContentLoaded', setupPageComponents); // Ensures the script runs after the document is fully loaded
})();