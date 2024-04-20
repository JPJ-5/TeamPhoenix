document.getElementById('create-collabRequest').addEventListener('click', function () {
    // get the username which is needed for gig creation.
    var feedbackBox = document.getElementById('create-gig-feedback');
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
                feedbackBox.textContent = 'Gig created successfully';
                feedbackBox.style.color = 'green';
                form.reset();
            } else {
                feedbackBox.textContent = 'Failed to create gig';
                feedbackBox.style.color = 'red';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            feedbackBox.textContent = 'Error creating gig. Please try again.';
            feedbackBox.style.color = 'red';
        });
    
        // Form is currently not displayed, so display it
        form.style.display = 'block';
    }
});
