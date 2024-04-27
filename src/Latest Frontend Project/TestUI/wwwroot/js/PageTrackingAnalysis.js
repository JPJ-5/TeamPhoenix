
// Method to track page visited/time spent.
var pageName = 'Home page'; //Name of the page the user is currently on.
let pageTimer;

// Function to start the page timer
function startPageTimer() {
    pageTimer = Date.now(); // Time page was first accessed
}

    // Function to reset the page timer
function resetPageTimer() {
    logPageDuration(pageName);
    startPageTimer();
}

// Function to log the session duration
// TODO: Change logging to be sent to database after done testing.
function logPageDuration(nameOfPage) {
    console.log(nameOfPage + ' duration: ' + (Date.now() - pageTimer) / 1000 + ' seconds'); //change to send this data back to database in a log later.
}

    // Function to check time after user moves pages.
function checkTimeUserSpentOnPage(nameOfPage) {
    logPageDuration(nameOfPage);
    resetPageTimer();
}