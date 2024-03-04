const express = require('express');
const app = express();
const cors = require('cors');
const mysql = require('mysql');
const path = require('path'); // Import path module

// Enable CORS for all routes
app.use(cors());

// MySQL database connection
const connection = mysql.createPool({
  host: '3.142.241.151',
  user: 'julie',
  password: 'j1234',
  database: 'MusiCali'
});

// Serve the index.html file

app.use(express.static('public'));

app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, 'index.html'));
});

// Endpoint to get all user data
app.get('/api/getallusers', (req, res) => {
    const query = 'SELECT Username, FirstName, LastName, DOB FROM UserProfile ORDER BY RAND() LIMIT 1';

    connection.query(query, (error, results) => {
        if (error) {
            return res.status(500).json({ error });
        }
        return res.status(200).json(results);
    });
});

const port = 5400;
app.listen(port, () => {
    console.log(`Server running on port ${port}`);
});