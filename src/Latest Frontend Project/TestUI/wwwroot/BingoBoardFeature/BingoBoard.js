var baseUrl = 'http://localhost:8080';
//var baseUrl = 'https://themusicali.com:5000';
document.getElementById('bbGoPrev').addEventListener('click', function(){
    var pageNum = sessionStorage.getItem('pageNum');
    pageNum--;
    console.log('loading page ', pageNum);
    sessionStorage.setItem('pageNum', pageNum);
    buildBingoBoard(pageNum);
})

document.getElementById('bbGoNext').addEventListener('click', function(){
    var pageNum = sessionStorage.getItem('pageNum');
    pageNum++;
    console.log('loading page ', pageNum);
    sessionStorage.setItem('pageNum', pageNum);
    buildBingoBoard(pageNum);
})

document.getElementById('bbPageSize').addEventListener('change', function(){
    var pageSize = document.getElementById('bbPageSize');
    sessionStorage.setItem('loadCount', pageSize.value);
    buildBingoBoard(1)
})

function buildBingoBoard(pageNum){
    clearBingoBoard();
    console.log("loading ", pageNum)
    const prevButton = document.getElementById('bbGoPrev');
    const nextButton = document.getElementById('bbGoNext');
    prevButton.style.display = 'none';
    nextButton.style.display = 'none';
    bingoBoardSize();
    //var tableSize = bingoBoardSize();
    //console.log(tableSize);
    document.querySelector('.main').style.display = 'none'; // Hide main content
    document.getElementById('BingoBoardView').style.display = 'block'; // Show bingo board
    //logFeatureUsage(username, "Bingo Board");

    const loadnotif = document.getElementById('BingoBoardLoadMsg');
    loadnotif.innerHTML = "Loading Posts...";

    var pageSize = sessionStorage.getItem('loadCount');
    var currentusername = sessionStorage.getItem('username');
    var idToken = sessionStorage.getItem('idToken');
    var accessToken = sessionStorage.getItem('accessToken');
    var offset = pageSize * (pageNum - 1);
    //configurePagination(pageSize, pageNum);
    
    //append additional post data to table html here
    BingoBoardUrl = baseUrl+'/BingoBoard/api/BingoBoardLoadGigs';
        fetch(BingoBoardUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authentication': idToken,
                'Authorization': accessToken
            },
            body: JSON.stringify({numberofgigs: pageSize, username: currentusername, offset: offset})
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

function bingoBoardSize(){
    const loadnotif = document.getElementById('BingoBoardLoadMsg');
    var idToken = sessionStorage.getItem('idToken');
    var accessToken = sessionStorage.getItem('accessToken');

    BingoBoardUrl = baseUrl+'/BingoBoard/api/BingoBoardRetrieveGigTableSize';
        fetch(BingoBoardUrl, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authentication': idToken,
                'Authorization': accessToken
            }
        })
            .then(response => {
                if (response.ok) {
                    return response.json();
                    
                } else {
                    throw new Error('Failed to retrieve gig table size');
                }
            })
            .then(bbSizeInt => {
                //console.log(bbSizeInt);
                sessionStorage.setItem('tableSize', bbSizeInt);
                configurePagination()
            })
            .catch(error => {
                console.error('Error:', error);
                loadnotif.innerHTML = "There was an error with the table. Please try again.";
            });
}

function loadBingoBoardPosts(page){
    clearBingoBoard();
    buildBingoBoard(page);
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

function configurePagination(){
    var tableSize = sessionStorage.getItem('tableSize');
    const minPage = 1
    const pageNumCurrent = sessionStorage.getItem('pageNum');
    const pageSize = sessionStorage.getItem('loadCount');
    var maxPage = Math.ceil(tableSize / pageSize)
    const prevButton = document.getElementById('bbGoPrev');
    const nextButton = document.getElementById('bbGoNext');
    //console.log(minPage, pageNumCurrent, maxPage);

    if(maxPage == minPage){
        prevButton.style.display = 'none';
        nextButton.style.display = 'none';
    }
    else if(pageNumCurrent <=1){
        prevButton.style.display = 'none';
        nextButton.style.display = 'block';
    }
    else if(maxPage > pageNumCurrent && pageNumCurrent > minPage){
        prevButton.style.display = 'block';
        nextButton.style.display = 'block';
    }
    else if(pageNumCurrent == maxPage){
        prevButton.style.display = 'block';
        nextButton.style.display = 'none';
    }
}

function setupPageComponents() {
    // Add any setup logic here
    if (sessionStorage.getItem('loadCount') == null) {
        sessionStorage.setItem('loadCount', 7);
    }
    sessionStorage.setItem('pageNum', 1)
    pageNum = sessionStorage.getItem('pageNum');
    buildBingoBoard(pageNum);
    console.log("Page components setup complete");
    sessionStorage.setItem('currentPage', 'BingoBoard')
}

// Call setupPageComponents when the page loads
window.onload = setupPageComponents;