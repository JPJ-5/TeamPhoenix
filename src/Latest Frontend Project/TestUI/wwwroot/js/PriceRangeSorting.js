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
        .then(data => displayResults(data.data))
        .catch(error => {
            console.error('Error fetching data:', error);
            alert(error.message);
        });
}

function displayResults(items) {
    const results = document.getElementById('results');
    results.innerHTML = '';
    if (items.length === 0) {
        results.innerHTML = '<p>No items found.</p>';
        return;
    }
    const ul = document.createElement('ul');
    items.forEach(item => {
        const li = document.createElement('li');
        li.textContent = `${item.name} - $${item.price.toFixed(2)}`;
        ul.appendChild(li);
    });
    results.appendChild(ul);
}

function setPredefinedRanges() {
    const range = document.getElementById('predefinedRanges').value;
    if (range) {
        const [min, max] = range.split('-');
        document.getElementById('bottomPrice').value = min;
        document.getElementById('topPrice').value = max;
    }
}

// Event listeners to reset the dropdown when manual price input is modified
document.getElementById('bottomPrice').addEventListener('input', function () {
    document.getElementById('predefinedRanges').value = ""; // Reset dropdown when manual change is made
});

document.getElementById('topPrice').addEventListener('input', function () {
    document.getElementById('predefinedRanges').value = ""; // Reset dropdown when manual change is made
});
