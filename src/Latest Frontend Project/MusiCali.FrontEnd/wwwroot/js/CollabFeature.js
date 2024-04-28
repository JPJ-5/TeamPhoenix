document.getElementById('enter-collabFeature').addEventListener('click', function () {


    //---------For testing purposes, will take hardcode away after-----------
    var feedbackBox = document.getElementById('enter-collabFeature');
    var sender = "kihambo.wav";
    var receiver = "juliereyes";
    //---------For testing purposes, will take hardcode away after-----------
    // Check if the form is already displayed
    {
    // Form is displayed, try to submit form data

    var payload = {
        senderUsername: sender,
        receiverUsername: receiver
    }

    fetch('http://localhost:8080/CollabFeature/api/LoadViewAPI', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(payload),
    })
        .then(response => response.json())
        .then(result => {
            feedbackBox.style.display = 'block';
            if (console.log(result)) {
                feedbackBox.textContent = 'View loaded successfully';
                feedbackBox.style.color = 'green';
                form.reset();
            } else {
                feedbackBox.textContent = 'Failed to load view';
                feedbackBox.style.color = 'red';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            feedbackBox.textContent = 'Error loading view. Please try again.';
            feedbackBox.style.color = 'red';
        });
    }
});


document.getElementById('create-collabRequest').addEventListener('click', function () {//listens for whenever an element with this id so whenever it's been clicked it'll run this function


    //---------For testing purposes, will take hardcode away after-----------
    var feedbackBox = document.getElementById('enter-collabFeature');
    var sender = "kihambo.wav";
    var receiver = "juliereyes";

    //---------For testing purposes, will take hardcode away after-----------
    // Check if the form is already displayed
    {
    // Form is displayed, try to submit form data

    var payload = {
        senderUsername: sender,
        receiverUsername: receiver
    }

    fetch('http://localhost:8080/CollabFeature/api/SendRequestAPI', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(payload),
    })
        .then(response => response.json())
        .then(result => {
            feedbackBox.style.display = 'block';
            if (console.log(result)) {
                feedbackBox.textContent = 'Collab request sent successfully';
                feedbackBox.style.color = 'green';
                form.reset();
            } else {
                feedbackBox.textContent = 'Failed to send collab';
                feedbackBox.style.color = 'red';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            feedbackBox.textContent = 'Error sending collab. Please try again.';
            feedbackBox.style.color = 'red';
        });
    }
});

document.getElementById('create-collabRequest').addEventListener('click', function () {//listens for whenever an element with this id so whenever it's been clicked it'll run this function
    //function collabSentAlert() {
        var popup = document.getElementById('myPopup');
        popup.classList.toggle("show");
        alert("Collab Request has been sent!");
    
});

document.getElementById('accept-collabRequest').addEventListener('click', function () {//listens for whenever an element with this id so whenever it's been clicked it'll run this function


    //---------For testing purposes, will take hardcode away after-----------
    var feedbackBox = document.getElementById('enter-collabFeature');
    var sender = "kihambo.wav";
    var receiver = "juliereyes";

    //---------For testing purposes, will take hardcode away after-----------
    // Check if the form is already displayed
    {
    // Form is displayed, try to submit form data

    var payload = {
        senderUsername: sender,
        receiverUsername: receiver
    }

    fetch('http://localhost:8080/CollabFeature/api/AcceptRequestAPI', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(payload),
    })
        .then(response => response.json())
        .then(result => {
            feedbackBox.style.display = 'block';
            if (console.log(result)) {
                feedbackBox.textContent = 'Collab request accepted successfully';
                feedbackBox.style.color = 'green';
                form.reset();
            } else {
                feedbackBox.textContent = 'Failed to accept collab';
                feedbackBox.style.color = 'red';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            feedbackBox.textContent = 'Error accepteing collab. Please try again.';
            feedbackBox.style.color = 'red';
        });
    }
});

document.getElementById('view-requests').addEventListener('click', function () {
    var feedbackBox = document.getElementById('collab-request-list');

    fetch('http://localhost:8080/CollabFeature/api/LoadCollabsAPI')
        .then(response => response.json())
        .then(result => {
            feedbackBox.innerHTML = ''; // Clear previous content
            if (result && result.length > 0) {
                result.forEach(collab => {
                    var collabElement = document.createElement('div');
                    collabElement.textContent = `Sender: ${collab.senderUsername}, Receiver: ${collab.receiverUsername}`;
                    feedbackBox.appendChild(collabElement);
                });
            } else {
                feedbackBox.textContent = 'No collab requests found.';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            feedbackBox.textContent = 'Error loading collab requests. Please try again.';
        });
});

function populateUserDropdown() {
    fetch('http://localhost:8080/CollabFeature/api/DisplayAvailableUsers')
        .then(response => response.json())
        .then(users => {
            const selectElement = document.getElementById('create-collabRequest');
            users.forEach(user => {
                const option = document.createElement('option');
                option.value = user.username;
                option.textContent = user.username;
                selectElement.appendChild(option);
            });
        })
        .catch(error => console.error('Error fetching authenticated users:', error));
}
document.addEventListener('DOMContentLoaded', function () {
    // Populate user dropdown when the page is loaded
    populateUserDropdown();
});

function displayCollabs() {
    var feedbackBox = document.getElementById('collab-request-list');

    var payload = {
        senderUsername: sender,
        receiverUsername: receiver
    }

    fetch('http://localhost:8080/CollabFeature/api/LoadCollabsAPI',{
    method: 'GET',
    headers: {
        'Content-Type': 'application/json',
    },
    body: JSON.stringify(payload),
})
        .then(function(response) {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(function(result) {
            feedbackBox.innerHTML = ''; // Clear previous content
            if (result && result.length > 0) {
                result.forEach(function(collab) {
                    var collabElement = document.createElement('div');
                    collabElement.textContent = 'Sender: ' + collab.senderUsername + ', Receiver: ' + collab.receiverUsername;
                    feedbackBox.appendChild(collabElement);
                });
            } else {
                feedbackBox.textContent = 'No collab requests found.';
            }
        })
        .catch(function(error) {
            console.error('Error:', error);
            feedbackBox.textContent = 'Error loading collab requests. Please try again.';
        });
}

function collabSentAlert() {
    var popup = document.getElementById('myPopup');
    popup.classList.toggle("show");
    alert("Collab Request has been sent!");
}

function acceptRequest(){
    var accept = document.getElementById('create-collabRequest');
    accept.classList.toggle("show");
    alert("You've accepted this collab!");
}








