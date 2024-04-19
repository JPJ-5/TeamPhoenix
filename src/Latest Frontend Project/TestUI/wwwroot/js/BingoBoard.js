var loadCount = 10;
var baseUrl = 'http://localhost:8080';

//Bingo Board Features:
document.getElementById('enter-BingoBoardView').addEventListener('click', function (){
    clearBingoBoard();
    buildBingoBoard(1);
});

document.getElementById('clearBingoBoard').addEventListener('click', function(){
    clearBingoBoard();
})

document.getElementById('loadBingoBoard').addEventListener('click', function(){
    loadBingoBoardPosts();
})

function buildBingoBoard(pageNum){
    sessionStorage.setItem('bbPage', pageNum);
    document.querySelector('.main').style.display = 'none'; // Hide main content
    document.getElementById('BingoBoardView').style.display = 'block'; // Show bingo board
    //logFeatureUsage(username, "Bingo Board");

    const loadnotif = document.getElementById('BingoBoardLoadMsg')
    loadnotif.innerHTML = "Loading Posts...";

    var currentusername = sessionStorage.getItem('username');
    var idToken = sessionStorage.getItem('idToken');
    var accessToken = sessionStorage.getItem('accessToken');
    var offset = loadCount * (pageNum - 1);

    
    //append additional post data to table html here
    BingoBoardUrl = baseUrl+'/BingoBoard/api/BingoBoardLoadGigs';
        fetch(BingoBoardUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authentication': idToken,
                'Authorization': accessToken
            },
            body: JSON.stringify({numberofgigs: loadCount, username: currentusername, offset: offset})
        })
            .then(response => {
                if (response.ok) {
                    //console.log(response.json());
                    return response.json();
                    
                } else {
                    throw new Error('Failed to load gigs');
                }
            })
            .then(gigSet => {
                constructGigList(gigSet);
            })
            .catch(error => {
                console.error('Error:', error);
                loadnotif.innerHTML = "There was an error with the table. Please try again.";
            });
}

function constructGigList(gigSet){
    const loadnotif = document.getElementById('BingoBoardLoadMsg')
    loadnotif.innerHTML = "";
    document.getElementById('BingoBoardPostsTable').style.display = 'block';
    var bbtable = document.getElementById('BingoBoardPostsTable');
    const gigData = gigSet.gigSummaries;//.values();
    console.log(gigData);
    for(i in (gigData)){
        //console.log(gigData[i]);
        var row = bbtable.insertRow();
        var titleCell = row.insertCell();
        var usernamecell = row.insertCell();
        var dateCell = row.insertCell();
        var locCell = row.insertCell();
        var payCell = row.insertCell();
        var descCell = row.insertCell();
        var interestButton = row.insertCell();

        titleCell.innerHTML = gigData[i].gigName;
        usernamecell.innerHTML = gigData[i].username;
        dateCell.innerHTML = gigData[i].dateOfGig;
        locCell.innerHTML = gigData[i].location;
        payCell.innerHTML=gigData[i].pay;
        descCell.innerHTML=gigData[i].description;
        var buttonID = "bingoButton"+gigData[i].gigID;
        interestButton.id = buttonID;
        interestButton.innerHTML="<input type='button' class='button' onclick='applyInterest("+gigData[i].gigID+");' value='Apply'/>";
    }
}

function loadBingoBoardPosts(){
    clearBingoBoard();
    buildBingoBoard(2);
}

function clearBingoBoard(){
    var bbtable = document.getElementById('BingoBoardPostsTable');
    for(let i = (bbtable.rows.length - 1); i >= 1; i--){
        bbtable.deleteRow(i);
    }
}

function applyInterest (id)
{
    var buttonID = 'bingoButton'+id
    var bingoButton = document.getElementById(buttonID);
    if(id>40){
    bingoButton.innerHTML = "<input type='button' class='button' id='"+buttonID+"' style = 'background-color: #3ba863; font-size: 14px;' value='   ✔   '/>"
    }
    else{
    bingoButton.innerHTML = "<input type='button' class='button' id='"+buttonID+"' style = 'background-color: #dc4545; font-size: 14px;' value='   ✘   '/>"
    }
    console.log(id);
    
}