const express = require('express');
const app = express();

app.post('/login', (req, res) => {
    // Perform user authentication here
    // If authentication is successful, redirect the user to the user modification page
    res.redirect('/UserModify.html');
});

// Serve the user modification page
app.get('/UserModify.html', (req, res) => {
    res.sendFile(__dirname + '/UserModify.html');
});

// Serve other static files
app.use(express.static(__dirname));

// Start the server
const port = 8080;
app.listen(port, () => {
    console.log(`Server is running on port ${port}`);
});
