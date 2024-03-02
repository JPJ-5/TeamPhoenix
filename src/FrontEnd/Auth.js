const express = require('express');
const app = express();

app.post('/login', (req, res) => {
    // Perform user authentication here
    // If authentication is successful, redirect the user to the user modification page
    res.redirect('/UserSettings.html');
});

// Serve the user modification page
app.get('/UserSettings.html', (req, res) => {
    res.sendFile(__dirname + '/UserSettings.html');
});

// Serve other static files
app.use(express.static(__dirname));

// Start the server
const port = 8080;
app.listen(port, () => {
    console.log(`Server is running on port ${port}`);
});
