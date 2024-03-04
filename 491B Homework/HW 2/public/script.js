function post(url, data) {
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
        body: JSON.stringify(data)
    };

    return fetch(url, options);
}

document.getElementById('postUserButton').addEventListener('click', function() {
    const url = `http://localhost:5400/api/postuserdata`;

    const userData = {
        // Example user data
        Username: 'thisisparthtest',
        FirstName: 'Parth',
        LastName: 'Thanki',
        DOB: '2000-02-21'
    };

    console.log("Posting data to:", url);

    post(url, userData)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log("Data posted:", data);
            // Optionally, update the UI or notify the user
        })
        .catch(error => {
            console.error('Error posting user data:', error);
        });
});

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

document.getElementById('getUserButton').addEventListener('click', function() {
  const url = `http://localhost:5400/api/getallusers`;

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
          // Display the user data
          document.getElementById('userData').innerText = JSON.stringify(data, null, 2);
      })
      .catch(error => {
          console.error('Error fetching user data:', error);
      });
});