const express = require('express')
const cors = require('cors')
const port = 8080;

const app = express();

app.use(cors())
app.use(function (request, response, next) {

    response.setHeader('Access-Control-Allow-Origin', 'http://localhost:3000');
    response.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, PATCH, DELETE');
    response.setHeader('Access-Control-Allow-Headers', '*'); // * means allow everything

    next();
})

// Web service endpoint setup
app.get('/', (request, response) => {
    response.setHeader('Content-Type', 'application/javascript; charset=UTF-8'); // For JSON MIME type payload

    var randomSize = Math.ceil(Math.random() * 10)
    var nameList = ["Edith", "Jason", "Iggy", "Paula", "Sera", "Jeff", "Tony", "Angela", "Nico", "Yuko"]; 
    var data = [];

    for (var i = 0; i < randomSize; i++) {
        var randomName = nameList[Math.floor(Math.random() * nameList.length)];

        data.push(randomName);
    }

    // Understanding the structure of the JSON data being returned to the front-end is critical for rendering dynamic content
    response.json({
        names: data
    });
});

// For preflight CORS request
app.options('/*', (request, response) => {
    response.sendStatus(200);
})

// Startup web server
app.listen(port, () => { console.log(`http://localhost:${port}`) });