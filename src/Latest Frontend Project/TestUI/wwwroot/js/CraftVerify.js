//var baseUrl = 'https://themusicali.com:5000';
var baseUrl = 'http://localhost:8080';



document.addEventListener('DOMContentLoaded', function () {
    var craftVerifyButton = document.getElementById('craftVerify');

    craftVerifyButton.addEventListener('click', function () {
        sessionStorage.setItem('currentPage', 'CraftVerify');
        loadCraftVerify();
    });

    function loadCraftVerify(){
        // Toggle visibility of the CraftVerify view
        document.querySelector('.main').style.display = 'none'; // Hide main content
        //document.getElementById('craftVerifyView').style.display = 'block'; // Show bingo board
        var view = document.getElementById('craftVerifyView');
        if (view.style.display === 'none') {
            view.style.display = 'block';
        } else {
            view.style.display = 'block';
        }
        //should call pricerange sorting here 
    }

});

function setupPagination(totalCount, itemsPerPage) {
    const pageCount = Math.ceil(totalCount / itemsPerPage);
    const pagination = document.getElementById('pagination');
    pagination.innerHTML = ''; // Clear previous links
    for (let i = 1; i <= pageCount; i++) {
        const pageLink = `<a href="#" onclick="fetchData(${i})">${i}</a> `;
        pagination.innerHTML += pageLink;
    }
}


window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}




document.addEventListener('DOMContentLoaded', function () {                                             // commented out 
    

    //document.getElementById('buyerHistoryBtn').addEventListener('click', function () {
    //    console.log('Buyer History clicked');
    //    // Additional functionality here
    //});


   

    //function disableSpecificButtonsIfMissingCredentials() {
    //    // Retrieve tokens and username from sessionStorage
    //    const jwtToken = sessionStorage.getItem('idToken');
    //    const accessToken = sessionStorage.getItem('accessToken');
    //    const username = sessionStorage.getItem('username');

    //    // Array of button IDs that require authentication
    //    const buttonIds = ['itemCreationBtn', 'itemModificationBtn', 'financialProgressBtn', 'pendingSaleBtn', 'offerPriceButton', 'buyButton']; // Replace these with your actual button IDs

    //    // Check each button by ID and disable if credentials are missing
    //    buttonIds.forEach(buttonId => {
    //        const button = document.getElementById(buttonId);
    //        if (button && (!jwtToken || !accessToken || !username)) {
    //            button.disabled = true;
    //            button.title = "Login required"; // Optional: Tooltip
    //        }
    //    });
    //}

    



    document.getElementById('sellerDashboardBtn').addEventListener('click', function () {
        // Hide the CraftVerify view
        var craftVerifyView = document.getElementById('craftVerifyView');
        craftVerifyView.style.display = 'none';

        // Show the Seller Dashboard view
        var sellerDashboardView = document.getElementById('sellerDashboardView');
        sellerDashboardView.style.display = 'block';

        // Optionally, initialize or refresh the Seller Dashboard contents
        //setupSellerDashboard(); // Ensure this function is defined to set up the dashboard
    });
});














