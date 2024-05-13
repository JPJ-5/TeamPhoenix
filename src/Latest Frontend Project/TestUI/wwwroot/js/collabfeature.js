//var baseUrl = 'https://themusicali.com:5000';
var baseUrl = 'http://localhost:8080';

document.addEventListener('DOMContentLoaded', function () {


    // Add event listener to "Send A Request" button
    document.getElementById('create-collabRequest').addEventListener('click', function() {
        sendCollabRequest();
    });

    // Add event listener to "Accept A Request" button
    document.getElementById('accept-collabRequest').addEventListener('click', function() {
        acceptCollabRequest();
    });

    // Add event listener to "View Collab Requests" button
    document.getElementById('view-requests').addEventListener('click', function() {
        displayCollabs();
    });

    //selection menu for which collabs the user wants to view
    document.getElementById('user-options').addEventListener('click', function(){
        updateCollabSelection();
    });

    document.getElementById('show-availUsers').addEventListener('click', function(){
        showAvailUsers();
    });

    function sendCollabRequest(collabUser)
    {
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
        fetch(`${baseUrl}/CollabFeature/api/SendRequestAPI`, {
method: 'POST',
            headers:
    {
        'Content-Type': 'application/json',
                'Authentication': idToken,
                'Authorization': accessToken
            },
            body: JSON.stringify(payload)
        })
        .then(response => {
             if (response.ok)
             {
                 return response.json();
             }
             else
             {
                 return response.json().then(json => { throw new Error(json.message); });
             }
         })
        .then(result => {
            feedbackBox.textContent = ''; // Clear previous messages
            if (result.success)
            {
                collabSentAlert();
                feedbackBox.textContent = 'Collab request sent successfully';
                feedbackBox.style.color = 'green';
            }
            else
            {
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
    function acceptCollabRequest()
{
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

        fetch(`${baseUrl}/CollabFeature/api/AcceptRequestAPI`, {
method: 'POST',
            headers:
    {
        'Content-Type': 'application/json',
                'Authentication': idToken,
                'Authorization': accessToken
            },
            body: JSON.stringify(payload),
        }) 
        .then(response => response.json())
        .then(result => {
            feedbackBox.style.display = 'block';
            if (result.success)
            {
                acceptCollabAlert();
                feedbackBox.textContent = 'Collab request accepted successfully';
                feedbackBox.style.color = 'green';
            }
            else
            {
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
    function displayCollabs()
{

    var selectedOption = document.getElementById('user-options').value;
    var username = sessionStorage.getItem("username");
    var feedbackBox = document.getElementById('enter-collabFeature');

    idToken = sessionStorage.getItem("idToken");
    accessToken = sessionStorage.getItem("accessToken");

        fetch(`${baseUrl}/CollabFeature/api/LoadCollabsAPI`,{
    method: 'GET',
            headers:
        {
            'Content-Type': 'application/json',
                'Authentication': idToken,
                'Authorization': accessToken,
                'userName': username
            },
        })
        .then(response => {
             if (response.ok)
             {
                 return response.json();

             }
             else
             {
                 return response.text().then(text => { throw new Error(text); });
             }
         })

        .then(collabData => {
            console.log(collabData)

            //feedbackBox.style.display = 'block';

            var displayNames = document.getElementById('user-options'); //append child
            var sentUsernames = document.createElement('p');

            //displayNames.innerHTML = ''; //clears previous details

            if (selectedOption === "View Sent Requests")
            {

                sentUsernames.textContent = `Sent Collabs:${ collabData.sentCollabs}`;
                alert("You've sent requests to: " + collabData.sentCollabs)
            }
            else if (selectedOption === "View Received Requests")
            {

                sentUsernames.textContent = `Received Collabs:${ collabData.receivedCollabs}`;
                alert("You received collabs from: " + collabData.receivedCollabs)
            }
            else if (selectedOption === "View Accepted Requests")
            {
                sentUsernames.textContent = `Accepted Collabs: ${ collabData.acceptedCollabs}`;
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
});

function showAvailUsers(userSearch)
{
    var table = document.getElementById("userTable");
    var username = sessionStorage.getItem("username");

    idToken = sessionStorage.getItem("idToken");
    accessToken = sessionStorage.getItem("accessToken");


    // Clear existing table content
    table.innerHTML = '';

    // Make AJAX request to backend API
    fetch(`${baseUrl}/CollabFeature/api/DisplayAvailableUsersAPI`,{
    method: 'GET',
            headers:
        {
            'Content-Type': 'application/json',
                'Authentication': idToken,
                'Authorization': accessToken,
                'userName': username
            },
        })
        .then(response => {
             if (response.ok)
             {
                 return response.json();
             }
             else
             {
                 throw new Error('Failed to fetch available users');
             }
         })
        .then(users => {
            // Create table rows for each user
            users.forEach(user => {
                var row = table.insertRow();
                var cell = row.insertCell();
                cell.textContent = user;
            });
        })
        .catch(error => {
            console.error('Error:', error);
            // Display error message or handle error appropriately
        });
    }

function collabSentAlert()
{
    var popup = document.getElementById('createPopup');
    var receiver = document.getElementById("receiver").value;

    popup.classList.toggle("show");
    alert("Collab Request has been sent to " + receiver + " !");
}

function acceptCollabAlert()
{
    var popup = document.getElementById('acceptPopup');
    var sender = document.getElementById('sender').value;

    popup.classList.toggle("show");
    alert("You accepted the collab request from " + sender + " !")
}



function updateCollabSelection()
{
    var selectBox = document.getElementById('user-options');
    var selectedIndex = selectBox.selectedIndex;

    var selectedValue = selectBox.options[selectedIndex];
    return selectedValue;

}