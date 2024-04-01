const express = require('express');
const app = express();
const mysql = require('mysql');
const path = require('path');

// Middleware to manually handle CORS
app.use((req, res, next) => {
    // Allow any domain to access your API
    res.header("Access-Control-Allow-Origin", "*");

    // Allow specific methods
    res.header("Access-Control-Allow-Methods", "GET, POST, OPTIONS, PUT, DELETE");

    // Allow specific headers
    res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");

    // Handle pre-flight requests for CORS
    if (req.method === "OPTIONS") {
        return res.status(200).end();
    }

    next();
});

// MySQL database connection
const connection = mysql.createPool({
  host: '3.142.241.151',
  user: 'julie',
  password: 'j1234',
  database: 'MusiCali'
});

// Middleware to parse JSON bodies
app.use(express.json());

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

// Endpoint to post user data
app.post('/api/postuserdata', (req, res) => {
    // Extract user data from request body
    const { Username, FirstName, LastName, DOB } = req.body;

    // SQL query to check if the user exists
    const checkUserExistsQuery = 'SELECT * FROM UserProfile WHERE Username = ?';

    // SQL query to update user data
    const updateUserQuery = 'UPDATE UserProfile SET FirstName = ?, LastName = ?, DOB = ? WHERE Username = ?';

    // First, check if the user exists
    connection.query(checkUserExistsQuery, [Username], (error, results) => {
        if (error) {
            console.error("SQL Error:", error);
            return res.status(500).json({ error: "Internal Server Error" });
        }
        
        if (results.length > 0) {
            // User exists, proceed with update
            connection.query(updateUserQuery, [FirstName, LastName, DOB, Username], (updateError, updateResults) => {
                if (updateError) {
                    console.error("SQL Error:", updateError);
                    return res.status(500).json({ error: "Error updating user data" });
                }
                return res.status(200).json({ message: 'User data updated successfully', data: req.body });
            });
        } else {
            // User does not exist, handle accordingly
            return res.status(404).json({ error: "User not found" });
        }
    });
});


const port = 8800;
app.listen(port, () => {
    console.log(`Server running on port ${port}`);
});