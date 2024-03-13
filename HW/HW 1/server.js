const express = require('express');
const app = express();
const cors = require('cors');
const mysql = require('mysql');
const path = require('path');
const port = 8080;
app.use(cors());

// MySQL database connection for CraftVerify and MusiCali
const connection = mysql.createPool({
  host: '3.142.241.151',
  user: 'julie',
  password: 'j1234',
  database: 'MusiCali'
});

app.use(express.static('public'));

app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, 'index.html'));
});

app.get('/api/getRandomHash', (req, res) => {
    const query = 'SELECT UserHash FROM UserAccount ORDER BY RAND() LIMIT 1';

    connection.query(query, (error, results) => {
        if (error) {
            return res.status(501).json({ error });
        }
        return res.status(200).json(results);
    });
});


app.listen(port, () => {
    console.log(`Server running on port ${port}`);
});