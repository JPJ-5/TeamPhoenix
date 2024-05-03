document.addEventListener('DOMContentLoaded', function () {
 

    // Add event listener to "Send A Request" button
    document.getElementById('create-collabRequest').addEventListener('click', function () {
        sendCollabRequest();
    });

    // Add event listener to "Accept A Request" button
    document.getElementById('accept-collabRequest').addEventListener('click', function () {
        acceptCollabRequest();
    });

    // Add event listener to "View Collab Requests" button
    document.getElementById('view-requests').addEventListener('click', function () {
        displayCollabs();
    });

    // Function t2o send collab request
    function sendCollabRequest(collabUser) {
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");

        var sender = document.getElementById("username").value;
        var receiver = document.getElementById("receiver").value;
        var feedbackBox = document.getElementById('enter-collabFeature');

        const payload = {
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
                collabSentAlert();
                feedbackBox.textContent = 'Collab request sent successfully';
                feedbackBox.style.color = 'green';
            } else {
                feedbackBox.textContent = 'Failed to send collab';
                feedbackBox.style.color = 'red';
            }
        })
        // .then(data => {
        //     console.log('Collaboration request has been sent to ' + receiver + ' !');
        // })
        .catch(error => {
            console.error('Error:', error);
            const feedbackBox = document.getElementById('enter-collabFeature');
            feedbackBox.textContent = 'Error sending collab. Please try again.';
            feedbackBox.style.color = 'red';
        });
    }

    // Function to accept collab request
    function acceptCollabRequest() {
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");

        var sender = document.getElementById("sender").value;
        var receiver = document.getElementById("username").value;
        var feedbackBox = document.getElementById('enter-collabFeature');

        const payload = {
            senderUsername: sender,
            receiverUsername: receiver
        };

        console.log(payload)

        fetch('http://localhost:8080/CollabFeature/api/AcceptRequestAPI', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authentication': idToken,
                'Authorization': accessToken
            },
            body: JSON.stringify(payload),
        }) 
        .then(response => response.json())
        .then(result => {
            feedbackBox = document.getElementById('enter-collabFeature');
            feedbackBox.style.display = 'block';
            if (result.success) {
                acceptCollabAlert();
                feedbackBox.textContent = 'Collab request accepted successfully';
                feedbackBox.style.color = 'green';
            } else {
                feedbackBox.textContent = 'Failed to accept collab';
                feedbackBox.style.color = 'red';
            }
        })
        // .then(data => {
        //     console.log('You accepted a collab request from ' + sender + ' !');
        // })
        .catch(error => {
            console.error('Error:', error);
            const feedbackBox = document.getElementById('enter-collabFeature');
            feedbackBox.textContent = 'Error accepting collab. Please try again.';
            feedbackBox.style.color = 'red';
        });
    }

    // Function to display collab requests
    function displayCollabs() {

        var username = sessionStorage.getItem("sender").value;
        var feedbackBox = document.getElementById('enter-collabFeature')

        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");

        fetch('http://localhost:8080/CollabFeature/api/LoadCollabsAPI',{
            method: 'GET',
            headers: {
                //'Content-Type': 'application/json',
                'Authentication': idToken,
                'Authorization': accessToken,
                'userName': username
            },
        })
        .then(response => {
            if (response.ok) {
                return response.json();
                
            } else {
                return response.text().then(text => { throw new Error(text); });
            }
        })
        // .then(collabData => {
        //     feedbackBox.style.display = 'block';
        //     if (result.success) {
        //         console.log(collabData)
        //         displayCollabData(collabData);
        //         feedbackBox.textContent = 'Sent collabs loaded successfully';
        //         feedbackBox.style.color = 'green';
        //     } 
        //     else {
        //         feedbackBox.textContent = 'Failed to load sent collabs';
        //         feedbackBox.style.color = 'red';
        //     }
        // })
        .then(collabData => {
            console.log(collabData)
            displayCollabData(collabData)
        })
        .catch(error => {
            console.error('Error:', error);
            feedbackBox.textContent = 'Error loading collab requests. Please try again.';
        });
    }
});

function displayCollabData(collabData){

    var sentUsernames = collabData.sentCollabs //list of sent collabs
    var receivedUsernames = collabData.receivedUsernames //list of received collabs
    var acceptedUsernames = collabData.acceptedUsernames //lsit of accepted collabs

    displayElement.textContent = sentUsernames;

}

function collabSentAlert() {
    var popup = document.getElementById('myPopup');
    var receiver = document.getElementById("receiver").value;

    popup.classList.toggle("show");
    alert("Collab Request has been sent to " + receiver + " !");
}

function acceptCollabAlert(){
    var popup = document.getElementById('myPopup');
    var sender = document.getElementById('sender').value;

    popup.classList.toggle("show");
    alert("You accepted the collab request from " + sender + " !")
}



//for later use
function moveLeft() {
    var selectBox = document.getElementById('user-options');
    if (!selectBox) return;
    var selectedIndex = selectBox.selectedIndex;
    if (selectedIndex > 0) {
        selectBox.selectedIndex = selectedIndex - 1;
        updateDisplay();
    }
}

function moveRight() {
    var selectBox = document.getElementById('user-options');
    if (!selectBox) return;
    var selectedIndex = selectBox.selectedIndex;
    if (selectedIndex < selectBox.options.length - 1) {
        selectBox.selectedIndex = selectedIndex + 1;
        updateDisplay();
    }
}