document.addEventListener('DOMContentLoaded', function () {
 

    //event listener for Send A Request button
    document.getElementById('create-collabRequest').addEventListener('click', function () {
        sendCollabRequest();
    });

    //event listener for Accept A Request button
    document.getElementById('accept-collabRequest').addEventListener('click', function () {
        acceptCollabRequest();
    });

    //event listener for View Collab Requests button
    document.getElementById('view-requests').addEventListener('click', function () {
        displayCollabs();
    });

    //selection menu for which collabs the user wants to view
    document.getElementById('user-options').addEventListener('click', function(){
        updateCollabSelection();
    });

    //event listener for displaying the table of available users
    document.getElementById('show-availUsers').addEventListener('click', function(){
        showAvailUsers();
    });
    
    //event listener for each button next to displayed user
    document.getElementById('userTable').addEventListener('click', function(event) {
        if (event.target.classList.contains('button')) {
            // If the clicked element has the class 'send-request-button'
            sendCollabRequest();
        }
    });

    //sending a request function
    function sendCollabRequest(user) {
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");
    
        var sender = document.getElementById("username").value;

        if(user != null){
            var receiver = user;
        }
        else{
            var receiver = document.getElementById("receiver").value;
        }
        console.log(receiver);
        var feedbackBox = document.getElementById('feedback-box');
    
        const payload = {
            senderUsername: sender,
            receiverUsername: receiver
        };
        console.log(payload)
        console.log("Sending collaboration request...");
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
            feedbackBox.textContent = ''; // clears previous messages
            if (result.success) {
                collabSentAlert();
                feedbackBox.textContent = 'Collab request sent successfully';
                feedbackBox.style.color = 'green';
            } else {
                feedbackBox.textContent = 'Collab Already Exists';
                feedbackBox.style.color = 'red';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            feedbackBox.textContent = 'Error sending collab. Please try again.';
            feedbackBox.style.color = 'red';
        });
    }
    

    // Function to accept collab request
    function acceptCollabRequest(user) {
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");

        var receiver = document.getElementById("username").value;
        var feedbackBox = document.getElementById('feedback-box');

        if(user != null){
            var sender = user;
        }
        else{
            var sender = document.getElementById("username").value;
        }
        const payload = {
            senderUsername: sender,
            receiverUsername: receiver
        };

        console.log(payload)
        console.log("Accepting collaboration request...");
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
        .catch(error => {
            console.error('Error:', error);
            const feedbackBox = document.getElementById('enter-collabFeature');
            feedbackBox.textContent = 'Error accepting collab. Please try again.';
            feedbackBox.style.color = 'red';
        });
    }


    //function to display sent, receiver, and accepted collab requests
    function displayCollabs() {

        var selectedOption = document.getElementById('user-options').value;
        var username = sessionStorage.getItem("username");
        var feedbackBox = document.getElementById('feedback-box');

        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");
        console.log("Retrieving collabs...");
        fetch('http://localhost:8080/CollabFeature/api/LoadCollabsAPI',{
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
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

        .then(collabData => {
            console.log(collabData)

            var displayNames = document.getElementById('user-options'); 
            var sentUsernames = document.createElement('p');

            if(selectedOption === "View Sent Requests"){ //view sent collabs

                sentUsernames.textContent = `Sent Collabs:${collabData.sentCollabs}`;
                alert("You've sent requests to: " + collabData.sentCollabs)
            }
            else if(selectedOption === "View Received Requests"){ //view received collabs

                sentUsernames.textContent = `Received Collabs:${collabData.receivedCollabs}`;
                alert("You received collabs from: " + collabData.receivedCollabs)
            }

            else if (selectedOption === "View Accepted Requests") { //view accepted collabs
                sentUsernames.textContent = `Accepted Collabs: ${collabData.acceptedCollabs}`;
                alert("You've accepted collab requests from: " + collabData.acceptedCollabs);
            }
            displayNames.appendChild(sentUsernames)
            displayNames.style.display = 'block';
        })
        .catch(error => {
            console.error('Error:', error);
            feedbackBox.textContent = 'Error loading collab requests. Please try again.';
            feedbackBox.style.color = 'red';
        });
    }

    //function for showing the available users to send and accept collabs in a table
    function showAvailUsers(userSearch){

        var table = document.getElementById("userTable");
        var username = sessionStorage.getItem("username");
        
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");
        

        // Clear existing table content
        table.innerHTML = '';

        // Make AJAX request to backend API
        fetch('http://localhost:8080/CollabFeature/api/DisplayAvailableUsersAPI',{
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authentication': idToken,
                'Authorization': accessToken,
                'userName': username
            },
        })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Failed to fetch available users');
            }
        })
        .then(users => {
            // creates a table of rows for each user
            users.forEach(user => {
                var row = table.insertRow();
                var usernameCell = row.insertCell();
                usernameCell.textContent = user;

                //adds "View Artist Portfolio" button next to each user
                var viewPortfolioButtonCell = row.insertCell();
                var viewPortfolioButton = document.createElement('button');
                viewPortfolioButton.textContent = 'View Artist Portfolio';
                viewPortfolioButton.onclick = function() {
                    // Handle button click event, e.g., navigate to artist portfolio page
                    alert('Viewing portfolio for ' + user);
                };
                viewPortfolioButtonCell.appendChild(viewPortfolioButton);

                //adds "Send a Request" button next to each user
                var sendRequestButtonCell = row.insertCell();
                var sendRequestButton = document.createElement('button');
                sendRequestButton.textContent = 'Send a Request';
                sendRequestButtonCell.appendChild(sendRequestButton);
                sendRequestButton.onclick = function() {
                    console.log(user)
                    sendCollabRequest(user);
                    alert("Collab Request has been sent to " + user + "!");
                };
 
                //adds "Accept a Request" button to each user
                var acceptRequestButtonCell = row.insertCell();
                var acceptRequestButton = document.createElement('button');
                acceptRequestButton.textContent = 'Accept a Request';
                acceptRequestButtonCell.appendChild(acceptRequestButton);
                acceptRequestButton.onclick = function() {
                    console.log(username) //testing to see if it displays anything
                    acceptCollabRequest(user);
                    alert("You accepted the collab request from  " + user + "!");
                };
            });
        })
        .catch(error => {
            console.error('Error:', error);

        });
    }



// function collabSentAlert() {
//     var popup = document.getElementById('createPopup');
//     var receiver = document.getElementById("receiver").value;

//     popup.classList.toggle("show");
//     alert("Collab Request has been sent to " + receiver + " !");
// }

// function acceptCollabAlert(){
//     var popup = document.getElementById('acceptPopup');
//     var sender = document.getElementById('sender').value;

//     popup.classList.toggle("show");
//     alert("You accepted the collab request from " + sender + " !")
// }


    //updates the type of collabs selection that the user wants to see
    function updateCollabSelection(){
        var selectBox = document.getElementById('user-options');
        var selectedIndex = selectBox.selectedIndex;

        var selectedValue = selectBox.options[selectedIndex];
        return selectedValue;

    }
    });
