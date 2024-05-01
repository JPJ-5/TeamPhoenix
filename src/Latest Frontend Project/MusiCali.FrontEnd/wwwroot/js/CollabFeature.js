document.addEventListener('DOMContentLoaded', function () {
 

    // Add event listener to "Send A Request" button
    document.getElementById('create-collabRequest').addEventListener('click', function () {
        sendCollabRequest();
        collabSentAlert();
    });

    // Add event listener to "Accept A Request" button
    document.getElementById('accept-collabRequest').addEventListener('click', function () {
        acceptCollabRequest();
    });

    // Add event listener to "View Collab Requests" button
    document.getElementById('view-requests').addEventListener('click', function () {
        displayCollabs();
    });ç

    // Function to send collab request
    function sendCollabRequest(collabUser) {
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");

        var sender = document.getElementById("username").value;
        var receiver = document.getElementById("receiver").value;
        var feedbackBox = document.getElementById('enter-collabFeature');

        const payload = {
            //CollabUser: collabUser,
            senderUsername: sender,
            receiverUsername: receiver
        };
        console.log(payload)
        fetch('http://localhost:8080/CollabFeature/api/SendRequestAPI', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authentication': idToken,
                'Authorization': accessToken
            },
            body: JSON.stringify(payload)
        })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                return response.json().then(json => { throw new Error(json.message); });
            }
        })
        .then(result => {
            feedbackBox.style.display = 'block';
            if (result.success) {
                feedbackBox.textContent = 'Collab request sent successfully';
                feedbackBox.style.color = 'green';
            } else {
                feedbackBox.textContent = 'Failed to send collab';
                feedbackBox.style.color = 'red';
            }
        })
        .then(data => {
            console.log('Collaboration Request has been sent');
        })
        .catch(error => {
            console.error('Error:', error);
            const feedbackBox = document.getElementById('enter-collabFeature');
            feedbackBox.textContent = 'Error sending collab. Please try again.';
            feedbackBox.style.color = 'red';
        });
    }

    // Function to accept collab request
    function acceptCollabRequest() {
        const payload = {
            senderUsername: username,
            receiverUsername: 'juliereyes' // Assuming receiver is fixed
        };
        fetch('http://localhost:8080/CollabFeature/api/AcceptRequestAPI', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(payload),
        }) 
        .then(response => response.json())
        .then(result => {
            const feedbackBox = document.getElementById('enter-collabFeature');
            feedbackBox.style.display = 'block';
            if (result.success) {
                feedbackBox.textContent = 'Collab request accepted successfully';
                feedbackBox.style.color = 'green';
            } else {
                feedbackBox.textContent = 'Failed to accept collab';
                feedbackBox.style.color = 'red';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            const feedbackBox = document.getElementById('enter-collabFeature');
            feedbackBox.textContent = 'Error accepting collab. Please try again.';
            feedbackBox.style.color = 'red';
        });
    }

    // Function to display collab requests
    function displayCollabs() {

        var sender = document.getElementById("username").value;

        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");

        const payload = {
            userName: sender
        };
        const feedbackBox = document.getElementById('view-requests');
        fetch(('http://localhost:8080/CollabFeature/api/LoadCollabsAPI'), {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authentication': idToken,
                'Authorization':accessToken
            },
            body: JSON.stringify(payload),
        })
        .then(response => {
            if (response.ok) {
                return response.json();
                
            } else {
                throw new Error('Failed to load collabs');
            }
        })
        .then(collabData => {
            displayCollabData(collabData)
        })
        .catch(error => {
            console.error('Error:', error);
            feedbackBox.textContent = 'Error loading collab requests. Please try again.';
        });
    }
});

function displayCollabData(data){

    var sentUsernames = data.sentCollabs //list of sent collabs
    var receivedUsernames = data.receivedUsernames //list of received collabs
    var acceptedUsernames = data.acceptedUsernames //lsit of accepted collabs

}

function collabSentAlert() {
    var popup = document.getElementById('myPopup');
    popup.classList.toggle("show");
    alert("Collab Request has been sent!");
}

document.querySelector("myPopup").addEventListener("click", collabSentAlert);