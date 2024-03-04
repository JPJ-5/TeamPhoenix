function get(url) {
    const options = {
        method: 'GET',
        mode: 'cors',
        cache: 'default',
        credentials: 'same-origin',
        headers: {
            'Content-Type': 'application/json'
        },
        redirect: 'follow',
        referrerPolicy: 'no-referrer-when-downgrade'
    };

    return fetch(url, options);
}

function send(url, data) {
    const options = {
        method: 'POST',
        mode: 'cors',
        cache: 'default',
        credentials: 'same-origin',
        headers: {
            'Content-Type': 'application/json'
        },
        redirect: 'follow',
        referrerPolicy: 'no-referrer-when-downgrade',
        body: ''
    };

    return fetch(url, options);
}

document.getElementById('getRandomEmail').addEventListener('click', function() {
    const url = `http://localhost:8800/api/getRandomEmail`;

    console.log("Sending request to:", url);

    get(url)
        .then(response => {
            console.log("Response received:", response);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log("Data received:", data);
            document.getElementById('randomEmail').innerText = JSON.stringify(data, null, 2);
        })
        .catch(error => {
            console.error('Error fetching user data:', error);
        });
});