document.getElementById('enter-collabFeature').addEventListener('click', function () {
    // get the username which is needed for gig creation.


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
    // get the username which is needed for gig creation.

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

document.getElementById('accept-collabRequest').addEventListener('click', function () {//listens for whenever an element with this id so whenever it's been clicked it'll run this function
    // get the username which is needed for gig creation.

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
