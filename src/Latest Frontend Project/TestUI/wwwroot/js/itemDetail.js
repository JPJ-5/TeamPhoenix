var baseUrl = 'https://themusicali.com:5000';
//var baseUrl = 'http://localhost:8080';

document.addEventListener('DOMContentLoaded', function () {
    const params = new URLSearchParams(window.location.search);
    const sku = params.get('sku');

    if (!sku) {
        document.getElementById('itemDetailsContainer').innerHTML = '<p>Error: No item specified.</p>';
        return;
    }

    fetchItemDetails(sku);
    //setupButtonHandlers(sku);
});

function setupButtonHandlers(sku) {
    document.getElementById('buyButton').addEventListener('click', function () {
        buyItem(sku);
    });

    document.getElementById('offerPriceButton').addEventListener('click', function () {
        offerPrice(sku);
    });

    document.getElementById('notifySellerButton').addEventListener('click', function () {
        notifySellerOutOfStock(sku);
    });
}

function buyItem(sku) {
    console.log(`Buying item with SKU: ${sku}`);
    // Implement buying logic, possibly making an API call
}

function offerPrice(sku) {
    console.log(`Making an offer for item with SKU: ${sku}`);
    // Could open a modal to input offer price, then make an API call
}

function notifySellerOutOfStock(sku) {
    console.log(`Notifying seller that item with SKU: ${sku} is out of stock`);
    // Make an API call to notify the seller
}


document.querySelectorAll('.thumbnail').forEach(item => {
    item.addEventListener('click', event => {
        const mainImage = document.getElementById('mainImage');
        mainImage.src = item.src; // Change the main image source to the thumbnail source
        mainImage.alt = item.alt; // Update alt text as well
    });
});



function fetchItemDetails(sku) {
    fetch(baseUrl + `/api/GetItemDetail?sku=${sku}`, { // Adjust the URL based on your actual API URL
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok ' + response.statusText);
            }
            return response.json();
        })
        .then(data => {
            updatePageContent(data);
        })
        .catch(error => {
            console.error('Error fetching item details:', error);
        });
}

function updatePageContent(item) {
    // Update main item information
    document.getElementById('itemName').textContent = item.name;
    document.getElementById('itemSKU').textContent = `SKU: ${item.sku}`;
    document.getElementById('itemPrice').textContent = `Price: $${item.price}`;
    document.getElementById('itemDescription').textContent = item.description;
    document.getElementById('itemStock').textContent = `Stock Available: ${item.stockAvailable}`;
    /*document.getElementById('itemSellerContact').textContent = item.sellerContact;*/

    // Update the main image
    const mainImage = document.getElementById('mainImage');
    mainImage.src = item.imageUrls[0]; // Ensure this is the main image URL
    

    // Ensure there is at least one URL for the main image
    if (item.imageUrls.length > 1) {
        // Update thumbnails starting from the second image URL
        item.imageUrls.slice(0).forEach((url, index) => {
            if (index < 5) { // Check if the index is within the number of available thumbnails
                const thumbnail = document.getElementById(`Image${index + 1}`);
                if (thumbnail) {
                    thumbnail.src = url;
                    thumbnail.alt = `Image ${index + 1} of ${item.name}`; // Note the index+2 for correct labeling
                    thumbnail.addEventListener('click', function () {
                        mainImage.src = thumbnail.src; // Update main image on thumbnail click
                        mainImage.alt = thumbnail.alt;
                    });
                }
            }
        });
    }

    // Update thumbnails for videos
    item.videoUrls.forEach((url, index) => {
        const videoThumbnail = document.getElementById(`Video${index + 1}`);
        if (videoThumbnail && videoThumbnail.children.length > 0) {
            videoThumbnail.children[0].src = url; // Assuming the first child is the <source> element
            videoThumbnail.load(); // Important to reload the video element to update the source
            videoThumbnail.onclick = () => {
                mainImage.style.display = 'none'; // Hide the main image
                videoThumbnail.style.display = 'block'; // Display the video
                videoThumbnail.play(); // Auto-play the video
            };
        }
    });
}




