document.getElementById('create-collabRequest').addEventListener('click', function () {
    // get the username which is needed for gig creation.
    var feedbackBox = document.getElementById('enter-collabFeature');
    var sender = "kihambo.wav";
    var receiver = "juliereyes";
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


document.getElementById('create-collabRequest').addEventListener('click', function () {
    // get the username which is needed for gig creation.
    var feedbackBox = document.getElementById('create-collabRequest');
    var sender = "kihambo.wav";
    var receiver = "juliereyes";
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
