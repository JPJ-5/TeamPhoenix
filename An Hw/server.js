const express = require('express');
const app = express();
const cors = require('cors');
const mysql = require('mysql');
const port = 8800;
const path = require('path');
app.use(cors());

const connection = mysql.createPool({
  host: '3.142.241.151',
  user: 'julie',
  password: 'j1234',
  database: 'MusiCali'
});

app.use(express.static('publicServer'));

app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, 'index.html'));
});

app.get('/api/getRandomEmail', (req, res) => {
    const query = 'SELECT Email FROM UserAccount ORDER BY RAND() LIMIT 1';

    connection.query(query, (error, results) => {
        if (error) {
            return res.status(502).json({ error });
        }
        return res.status(200).json(results);
    });
});

app.listen(port, () => {
    console.log(`Server running on port ${port}`);
});