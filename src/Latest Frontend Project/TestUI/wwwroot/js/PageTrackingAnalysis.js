var baseUrl = 'http://localhost:8080';
//var baseUrl = 'https://themusicali.com:5000';

// Method to track page visited/time spent.
var pageName = 'Home page'; //Name of the page the user is currently on.
let pageTimer;

// Function to start the page timer
function startPageTimer() {
    pageTimer = Date.now(); // Time page was first accessed
}

// Function to reset the page timer
function resetPageTimer(newNameOfPage) {
    logPageDuration(pageName);
    pageName = newNameOfPage;
    startPageTimer();
}

// Function to log the session duration
function logPageDuration(nameOfPage) {
    var username = document.getElementById("username").value;
    var pageLengthInMilliseconds = Math.floor(Date.now() - pageTimer);
    var url = `${baseUrl}/UsageAnalysisDashboard/api/UsageAnalysisDashboardLogPageLengthAPI`;

    var data = {
        Username: username,
        Context: nameOfPage,
        PageLength: pageLengthInMilliseconds
    };

    idToken = sessionStorage.getItem("idToken");
    accessToken = sessionStorage.getItem("accessToken");

    var options = {
        method: 'POST',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    };

    fetch(url, options)
        .then(response => response.json())
        .catch(error => {
            console.error('Error sending data:', error);
        });
}