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
        .then(data => displayResults(data))
        .catch(error => {
            console.error('Error fetching data:', error);
            results.innerHTML = `<p>Error: ${error.message}</p>`;
        })
        .finally(() => {
            loadingIndicator.style.display = 'none';
        });
}

function displayResults(items) {
    const results = document.getElementById('results');
    results.innerHTML = '';
    if (items.length === 0) {
        results.innerHTML = '<p>No items found within the specified price range.</p>';
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