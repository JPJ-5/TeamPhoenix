



document.addEventListener('DOMContentLoaded', function () {
    const params = new URLSearchParams(window.location.search);
    const sku = params.get('sku');

    // Setup handlers and other initializations that do not require the SKU
    setupNonSkuPageHandlers(); // Example function for general setup

    if (!sku) {
        console.log('No SKU provided. Certain functionalities will be disabled.');
    } else {
        fetchListingItem(sku); // Fetch details only if SKU is available
        setupNonSkuPageHandlers(sku); // Setup button handlers that require the SKU
    }
});

function setupNonSkuPageHandlers() {
    // Setup navigation links or buttons
    document.querySelectorAll('.nav-button').forEach(button => {
        button.addEventListener('click', function () {
            const section = document.querySelector(this.getAttribute('data-target'));
            if (section) {
                section.scrollIntoView({ behavior: 'smooth' });
            }
        });
    });

    // Initialize any tooltips on the page
    document.querySelectorAll('[data-tooltip]').forEach(element => {
        element.addEventListener('mouseenter', function () {
            const tooltipText = this.getAttribute('data-tooltip');
            const tooltip = document.createElement('div');
            tooltip.className = 'tooltip';
            tooltip.textContent = tooltipText;
            this.appendChild(tooltip);
            tooltip.style.left = this.offsetWidth / 2 - tooltip.offsetWidth / 2 + 'px'; // Center the tooltip
            tooltip.style.top = this.offsetTop - tooltip.offsetHeight - 10 + 'px'; // Position above the element
        });

        element.addEventListener('mouseleave', function () {
            this.removeChild(this.querySelector('.tooltip'));
        });
    });

    // Prepare modal windows
    document.querySelectorAll('.modal-trigger').forEach(trigger => {
        trigger.addEventListener('click', function () {
            const modalId = this.getAttribute('data-modal');
            const modal = document.getElementById(modalId);
            if (modal) {
                modal.style.display = 'block';
            }
        });
    });

    // Close modals when clicking on close buttons
    document.querySelectorAll('.modal-close').forEach(closeButton => {
        closeButton.addEventListener('click', function () {
            this.closest('.modal').style.display = 'none';
        });
    });

    // Maybe add more general event listeners or setups as needed
}

document.addEventListener('DOMContentLoaded', setupNonSkuPageHandlers);


document.getElementById('listedItemsBtn').addEventListener('click', function () {
    hideAllSections();
    document.getElementById('itemsListingContainer').style.display = 'block';
    fetchListingItem(1); // Fetch and populate the items
   
    
});
function fetchListingItem(page) {
    const username = sessionStorage.getItem('username');
    const token = sessionStorage.getItem('token'); // Assuming you use token-based authentication
    const headers = new Headers({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    });

    const url = `${baseUrl}/api/ItemPagination?listed=true&pageNumber=${page}&pageSize=10`;

    fetch(url, { headers: headers })
        .then(response => {
            if (!response.ok) throw new Error('Failed to fetch data');
            return response.json();
        })
        .then(data => {

            renderListingItems(data.items);
            setupPaginationItemListing('fetchListingItem', data.totalCount, 10, page);
        })
        .catch(error => {
            console.error('Error fetching data:', error);
            alert('Failed to load items. Please try again.');
        });
}

function renderListingItems(items) {
    const tbody = document.getElementById('allItemsTableBody');
    if (!tbody) return;
    tbody.innerHTML = ''; // Clear previous items

    items.forEach(item => {
        const imageUrl = item.firstImage || 'css/default_image.jpg';

        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${item.name}</td>
            <td>$${item.price.toFixed(2)}</td>
            <td><img src="${imageUrl}" alt="${item.name}" style="width: 100px;"></td>
            <td>${item.stockAvailable}</td>
        `;

        // Add a row click event to navigate to the item detail page or open a modal
        row.addEventListener('click', () => {
            window.location.href = `itemDetail.html?sku=${item.sku}`;
        });

        tbody.appendChild(row);
    });
}

function setupPaginationItemListing(fetchFunction, totalCount, pageSize, currentPage) {
    const totalPages = Math.ceil(totalCount / pageSize);
    const paginationContainer = document.getElementById('pagination');
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