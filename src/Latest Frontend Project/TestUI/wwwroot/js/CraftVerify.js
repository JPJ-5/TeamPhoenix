//var baseUrl = 'https://themusicali.com:5000';
var baseUrl = 'http://localhost:8080';

//function disableSpecificButtonsIfMissingCredentials() {
//    // Retrieve tokens and username from sessionStorage
//    const jwtToken = sessionStorage.getItem('idToken');
//    const accessToken = sessionStorage.getItem('accessToken');
//    const username = sessionStorage.getItem('username');

//    const restrictedElements = ['financialProgressReportView', 'creationForm', 'pendingSaleContainer','itemModificationForm']; // Add your specific view IDs here
//    restrictedElements.forEach(elementId => {
//        const element = document.getElementById(elementId);
//        if (!jwtToken || !accessToken || !username) {
//            if (element) {
//                element.style.display = 'none'; // Hide views that require authentication
//            }
//        } else {
//            if (element) {
//                element.style.display = 'block'; // Show view if authenticated
//            }
//        }
//    });

//    // Array of button IDs that require authentication
//    const buttonIds = ['itemCreationBtn', 'itemModificationBtn', 'financialProgressBtn', 'pendingSaleBtn',
//        'offerPriceButton', 'buyButton', 'fetchYearly', 'fetchQuarterly', 'fetchMonthly']; // Replace these with your actual button IDs

//    // Check each button by ID and disable if credentials are missing
//    buttonIds.forEach(buttonId => {
//        const button = document.getElementById(buttonId);
//        if (button) {
//            if (!jwtToken || !accessToken || !username) {// Add click event listener that checks credentials upon click
//            button.disabled = true;

//            }
//            else
//                button.disabled = false;
//        }
//    });
//}
//document.addEventListener('DOMContentLoaded', disableSpecificButtonsIfMissingCredentials);



//document.addEventListener('DOMContentLoaded', function () {


document.getElementById('craftVerify').addEventListener('click', function () {

    document.querySelector('.main').style.display = 'none'; // Hide main content

    var craftVerifyView1 = document.getElementById('craftVerifyView');   //show craft verify view
    craftVerifyView1.style.display = 'block';

});

document.getElementById('sellerDashboardBtn').addEventListener('click', function () {


    var craftVerifyView = document.getElementById('craftVerifyView');  // Hide the CraftVerify view
    craftVerifyView.style.display = 'none';
    hideAllSectionsFromCraft();  // hide all other views

    var sellerDashboardView = document.getElementById('sellerDashboardView'); // Show the Seller Dashboard view
    sellerDashboardView.style.display = 'block';

    const jwtToken = sessionStorage.getItem('idToken');              // check if user is logged in or not
    const accessToken = sessionStorage.getItem('accessToken');
    const username = sessionStorage.getItem('username');
    if (!jwtToken || !accessToken || !username) {
        alert("Non Login User!!! Several features will be restricted. Please click on the right menu button to log in");
    } else {
        document.getElementById('itemCreationBtn').style.display = 'block';
        document.getElementById('itemModificationBtn').style.display = 'block';
        document.getElementById('financialProgressBtn').style.display = 'block';
        document.getElementById('pendingSaleBtn').style.display = 'block';
    }
});











