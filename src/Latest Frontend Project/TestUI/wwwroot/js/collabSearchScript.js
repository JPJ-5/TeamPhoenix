document.getElementById('searchButton').addEventListener('click', async function () {
    const username = document.getElementById('searchInput').value.trim();

    try {
        // Send search request to backend API
        const response = await fetch('http://localhost:8080/CollabSearch/api/searchAPI', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username: username })
        });

        const data = await response.json();

        // Display search results
        const resultsBox = document.getElementById('resultsBox');
        resultsBox.innerHTML = ''; // Clear previous results

        if (data.length === 0) {
            resultsBox.textContent = 'No results found';
        } else {
            const resultsList = document.createElement('ul');
            data.forEach(user => {
                const listItem = document.createElement('li');
                listItem.textContent = user.username;

                // Create button for requesting collab
                const requestButton = document.createElement('button');
                requestButton.textContent = 'Request Collab';
                requestButton.className = 'user-button';
                listItem.appendChild(requestButton);

                resultsList.appendChild(listItem);
            });
            resultsBox.appendChild(resultsList);
        }
    } catch (error) {
        console.error('Error searching users:', error);
        // Handle error (e.g., display error message to user)
    }
});
