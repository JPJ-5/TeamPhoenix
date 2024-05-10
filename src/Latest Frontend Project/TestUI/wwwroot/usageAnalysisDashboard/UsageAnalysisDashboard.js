let monthsInTimespan = null;
function setMonthsInTimespan() { 
    monthsInTimespan = document.getElementById('monthsInTimespan').value
}

function getAllChartsInTimespan() {
    getLoginInTimespan();
    getRegistrationInTimespan();
    getGigsCreatedInTimespan();
    getItemsSoldInTimespan();
    getPageLengthInTimespan();
}
function getLoginInTimespan() {
    var results = document.getElementById('login-results-container')
    var username = document.getElementById("username").value;
    monthsInTimespan = document.getElementById('monthsInTimespan').value
    let url = `http://localhost:8080/UsageAnalysisDashboard/api/UsageAnalysisDashboardGetLoginAPI?username=${username}&monthsInTimeSpan=${monthsInTimespan}`;
    fetch(url)
        .then(response => response.json())
        .then(data => {
            displayLoginResults(data);
            results.innerHTML = ``;
        }).catch(error => {
            results.innerHTML = `<p>Error: ${error.message}</p>`;
        });
}

function displayLoginResults(loginData) {
    var data = {
        labels: loginData.months,
        values: loginData.count
    };

    // Reference to the chart container
    var chartContainer = document.getElementById('login-chart-container');

    // Remove existing chart from the chart container
    var existingCanvas = chartContainer.querySelector('canvas');
    if (existingCanvas) {
        chartContainer.removeChild(existingCanvas);
    }

    // Create canvas element for the chart
    var canvas = document.createElement('canvas');
    canvas.width = 400;
    canvas.height = 300;
    chartContainer.appendChild(canvas);

    // Get 2D context of the canvas
    var ctx = canvas.getContext('2d');

    // write title
    var title = "Login Data in the past " + monthsInTimespan + " months";
    ctx.font = "12px Arial";
    ctx.fillText(title, 50, 30);

    // Draw the axes
    ctx.beginPath();
    ctx.moveTo(50, 20);
    ctx.lineTo(50, 250);
    ctx.lineTo(350, 250);
    ctx.stroke();

    var formattedLabels = data.labels.map(function (dateTime) {
        var date = new Date(dateTime);
        return date.toLocaleString('default', { month: 'short', year: 'numeric' });
    });

    // Draw the labels on the x-axis
    for (var i = 0; i < formattedLabels.length; i++) {
        ctx.fillText(formattedLabels[i], 50 + i * 50, 270);
    }

    // Draw the labels on the y-axis

    // get the maximum value of the data
    var maxCount = Math.max(...data.values); //elipisis separates each element in the list so it can find the highest value. 
    var stepY = 200 / maxCount;
    var yLabelStep = Math.ceil(maxCount / 10); // Used to space out the y labels to be readable.
    for (var j = 0; j <= maxCount; j += yLabelStep) {
        ctx.fillText(j, 30, 250 - j * stepY);
    }

    // Plot the data points
    ctx.beginPath();
    ctx.moveTo(50, 250 - data.values[0] * stepY);
    for (var k = 1; k < data.values.length; k++) {
        ctx.lineTo(50 + k * 50, 250 - data.values[k] * stepY);
    }
    ctx.strokeStyle = 'blue';
    ctx.lineWidth = 2;
    ctx.stroke();
}

function setupLoginBoard() {
    var chartContainer = document.getElementById('login-chart-container');
    var canvas = document.createElement('canvas');
    canvas.width = 400;
    canvas.height = 300;
    chartContainer.appendChild(canvas);
}

function getRegistrationInTimespan() {
    var results = document.getElementById('registration-results-container')
    var username = document.getElementById("username").value;
    monthsInTimespan = document.getElementById('monthsInTimespan').value
    let url = `http://localhost:8080/UsageAnalysisDashboard/api/UsageAnalysisDashboardGetRegistrationAPI?username=${username}&monthsInTimeSpan=${monthsInTimespan}`;
    fetch(url)
        .then(response => response.json())
        .then(data => {
            displayRegistrationResults(data);
            results.innerHTML = ``;
        }).catch(error => {
            results.innerHTML = `<p>Error: ${error.message}</p>`;
        });
}

function displayRegistrationResults(registrationData) {
    var data = {
        labels: registrationData.months,
        values: registrationData.count
    };

    // Reference to the chart container
    var chartContainer = document.getElementById('registration-chart-container');

    // Remove existing chart from the chart container
    var existingCanvas = chartContainer.querySelector('canvas');
    if (existingCanvas) {
        chartContainer.removeChild(existingCanvas);
    }

    // Create canvas element for the chart
    var canvas = document.createElement('canvas');
    canvas.width = 400;
    canvas.height = 300;
    chartContainer.appendChild(canvas);

    // Get 2D context of the canvas
    var ctx = canvas.getContext('2d');

    // write title
    var title = "Registration Data in the past " + monthsInTimespan + " months";
    ctx.font = "12px Arial";
    ctx.fillText(title, 50, 30);

    // Draw the axes
    ctx.beginPath();
    ctx.moveTo(50, 20);
    ctx.lineTo(50, 250);
    ctx.lineTo(350, 250);
    ctx.stroke();

    var formattedLabels = data.labels.map(function (dateTime) {
        var date = new Date(dateTime);
        return date.toLocaleString('default', { month: 'short', year: 'numeric' });
    });

    // Draw the labels on the x-axis
    for (var i = 0; i < formattedLabels.length; i++) {
        ctx.fillText(formattedLabels[i], 50 + i * 50, 270);
    }

    // Draw the labels on the y-axis
    // get the maximum value of the data
    var maxCount = Math.max(...data.values); //elipisis separates each element in the list so it can find the highest value. 
    var stepY = 200 / maxCount;
    var yLabelStep = Math.ceil(maxCount / 10); // Used to space out the y labels to be readable.
    for (var j = 0; j <= maxCount; j += yLabelStep) {
        ctx.fillText(j, 30, 250 - j * stepY);
    }

    // Plot the data points
    ctx.beginPath();
    ctx.moveTo(50, 250 - data.values[0] * stepY);
    for (var k = 1; k < data.values.length; k++) {
        ctx.lineTo(50 + k * 50, 250 - data.values[k] * stepY);
    }
    ctx.strokeStyle = 'blue';
    ctx.lineWidth = 2;
    ctx.stroke();
}

function setupRegistrationBoard() {
    var chartContainer = document.getElementById('registration-chart-container');
    var canvas = document.createElement('canvas');
    canvas.width = 400;
    canvas.height = 300;
    chartContainer.appendChild(canvas);
}

function getGigsCreatedInTimespan() {
    var results = document.getElementById('gigs-created-results-container')
    var username = document.getElementById("username").value;
    monthsInTimespan = document.getElementById('monthsInTimespan').value
    let url = `http://localhost:8080/UsageAnalysisDashboard/api/UsageAnalysisDashboardGetGigsCreatedAPI?username=${username}&monthsInTimeSpan=${monthsInTimespan}`;
    fetch(url)
        .then(response => response.json())
        .then(data => {
            displayGigsCreatedResults(data);
            results.innerHTML = ``;
        }).catch(error => {
            results.innerHTML = `<p>Error: ${error.message}</p>`;
        });
}

function displayGigsCreatedResults(gigsCreatedData) {
    var data = {
        labels: gigsCreatedData.months,
        values: gigsCreatedData.count
    };

    // Reference to the chart container
    var chartContainer = document.getElementById('gigs-created-chart-container');

    // Remove existing chart from the chart container
    var existingCanvas = chartContainer.querySelector('canvas');
    if (existingCanvas) {
        chartContainer.removeChild(existingCanvas);
    }

    // Create canvas element for the chart
    var canvas = document.createElement('canvas');
    canvas.width = 400;
    canvas.height = 300;
    chartContainer.appendChild(canvas);

    // Get 2D context of the canvas
    var ctx = canvas.getContext('2d');

    // write title
    var title = "Gigs Created in the past " + monthsInTimespan + " months";
    ctx.font = "12px Arial";
    ctx.fillText(title, 50, 30);

    // Draw the axes
    ctx.beginPath();
    ctx.moveTo(50, 20);
    ctx.lineTo(50, 250);
    ctx.lineTo(350, 250);
    ctx.stroke();

    var formattedLabels = data.labels.map(function (dateTime) {
        var date = new Date(dateTime);
        return date.toLocaleString('default', { month: 'short', year: 'numeric' });
    });

    // Draw the labels on the x-axis
    for (var i = 0; i < formattedLabels.length; i++) {
        ctx.fillText(formattedLabels[i], 50 + i * 50, 270);
    }

    // Draw the labels on the y-axis
    // get the maximum value of the data
    var maxCount = Math.max(...data.values); //elipisis separates each element in the list so it can find the highest value. 
    var stepY = 200 / maxCount;
    var yLabelStep = Math.ceil(maxCount / 10); // Used to space out the y labels to be readable.
    for (var j = 0; j <= maxCount; j += yLabelStep) {
        ctx.fillText(j, 30, 250 - j * stepY);
    }

    // Plot the data points
    ctx.beginPath();
    ctx.moveTo(50, 250 - data.values[0] * stepY);
    for (var k = 1; k < data.values.length; k++) {
        ctx.lineTo(50 + k * 50, 250 - data.values[k] * stepY);
    }
    ctx.strokeStyle = 'blue';
    ctx.lineWidth = 2;
    ctx.stroke();
}

function setupGigsCreatedBoard() {
    var chartContainer = document.getElementById('gigs-created-chart-container');
    var canvas = document.createElement('canvas');
    canvas.width = 400;
    canvas.height = 300;
    chartContainer.appendChild(canvas);
}

function getItemsSoldInTimespan() {
    var results = document.getElementById('items-sold-results-container')
    var username = document.getElementById("username").value;
    monthsInTimespan = document.getElementById('monthsInTimespan').value
    let url = `http://localhost:8080/UsageAnalysisDashboard/api/UsageAnalysisDashboardGetItemsSoldAPI?username=${username}&monthsInTimeSpan=${monthsInTimespan}`;
    fetch(url)
        .then(response => response.json())
        .then(data => {
            displayItemsSoldResults(data);
            results.innerHTML = ``;
        }).catch(error => {
            results.innerHTML = `<p>Error: ${error.message}</p>`;
        });
}
function displayItemsSoldResults(itemsSoldData) {
    var textContainer = document.getElementById('items-sold-text-container');

    // Clear existing content of the element
    textContainer.innerHTML = '';

    for (var i = 0; i < itemsSoldData.itemNames.length; i++) {
        var topPlacementNumber = i + 1;
        var textNode = document.createTextNode(topPlacementNumber + ". Item name: " + itemsSoldData.itemNames[i] + " Quantity Sold: " + itemsSoldData.quantity[i] + " ");
        textContainer.appendChild(textNode);
    }
}

function getPageLengthInTimespan() {
    var results = document.getElementById('page-length-results-container')
    var username = document.getElementById("username").value;
    monthsInTimespan = document.getElementById('monthsInTimespan').value
    let url = `http://localhost:8080/UsageAnalysisDashboard/api/UsageAnalysisDashboardGetLongestPageViewAPI?username=${username}&monthsInTimeSpan=${monthsInTimespan}`;
    fetch(url)
        .then(response => response.json())
        .then(data => {
            displayPageLengthResults(data);
            results.innerHTML = ``;
        }).catch(error => {
            results.innerHTML = `<p>Error: ${error.message}</p>`;
        });
}
function displayPageLengthResults(pageLengthData) {
    var textContainer = document.getElementById('page-length-text-container');

    // Clear existing content of the element
    textContainer.innerHTML = '';

    for (var i = 0; i < pageLengthData.pageNames.length; i++) {
        var topPlacementNumber = i + 1;
        var lengthOfPageInSeconds = pageLengthData.lengthOfPage[i] / 1000;
        var textNode = document.createTextNode(topPlacementNumber + ". Name of Page View: " + pageLengthData.pageNames[i] + " Average Length of Time: " + lengthOfPageInSeconds + " ");
        textContainer.appendChild(textNode);
    }
}

function setupUsageAnalysisDashboard() {
    var username = document.getElementById("username").value;
    setupLoginBoard();
    setupRegistrationBoard();
    setupGigsCreatedBoard();
}

setInterval(getAllChartsInTimespan, 60000);