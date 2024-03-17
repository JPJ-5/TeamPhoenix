<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Collab Search</title>
    <style>
        /* Add your CSS styles here */
        body {
            font-family: Arial, sans-serif;
            background-color: #f0f0f0;
            margin: 0;
            padding: 0;
        }
        .container {
            max-width: 800px;
            margin: 50px auto;
            padding: 20px;
            background-color: #fff;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }
        h1 {
            text-align: center;
            margin-bottom: 20px;
        }
        .search-form {
            display: flex;
            justify-content: center;
            align-items: center;
        }
        .search-input {
            width: 300px;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 4px;
            margin-right: 10px;
            font-size: 16px;
        }
        .search-button {
            padding: 10px 20px;
            background-color: #007bff;
            color: #fff;
            border: none;
            border-radius: 4px;
            font-size: 16px;
            cursor: pointer;
        }
        .search-button:hover {
            background-color: #0056b3;
        }
        .results-box {
            margin-top: 20px;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 4px;
            background-color: #f9f9f9;
        }
    </style>
</head>
<body>
    <div class="container">
        <h1>Collab Search</h1>
        <div class="search-form">
            <input type="text" id="searchInput" class="search-input" placeholder="Search by username">
            <button id="searchButton" class="search-button">Search</button>
        </div>
        <div id="resultsBox" class="results-box"></div>
    </div>

    <script>
        // Function to handle form submission
        document.getElementById('searchButton').addEventListener('click', async function() {
            const username = document.getElementById('searchInput').value.trim();

            try {
                // Send search request to backend API
                const response = await fetch(`/search?username=${username}`);
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
                        resultsList.appendChild(listItem);
                    });
                    resultsBox.appendChild(resultsList);
                }
            } catch (error) {
                console.error('Error searching users:', error);
                // Handle error (e.g., display error message to user)
            }
        });
    </script>
</body>
</html>
