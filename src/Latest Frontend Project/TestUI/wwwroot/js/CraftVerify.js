var baseUrl = 'https://themusicali.com:5000';
//var baseUrl = 'http://localhost:8080';

document.addEventListener('DOMContentLoaded', function () {
    var craftVerifyButton = document.getElementById('craftVerify');

    craftVerifyButton.addEventListener('click', function () {
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
    });

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

document.addEventListener('DOMContentLoaded', function () { 

    document.getElementById('sellerDashboardBtn').addEventListener('click', function () {
        // Hide the CraftVerify view
        var craftVerifyView = document.getElementById('craftVerifyView');
        craftVerifyView.style.display = 'none';

        // Show the Seller Dashboard view
        var sellerDashboardView = document.getElementById('sellerDashboardView');
        sellerDashboardView.style.display = 'block';
    });
});














