
//var baseUrl = 'https://themusicali.com:5000';
var baseUrl = 'http://localhost:8080';

document.addEventListener('DOMContentLoaded', function () {
    const params = new URLSearchParams(window.location.search);
    const sku = params.get('sku');

    if (!sku) {
        document.getElementById('itemDetailsContainer').innerHTML = '<p>Error: No item specified.</p>';
        return;
    }

    fetchItemDetails(sku);
    setupButtonHandlers(sku);
});




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

function processItemDetails(item) {
    const offerablePrice = item.offerablePrice;
    const offerContainer = document.getElementById('itemOfferContainer');

    offerContainer.style.display = offerablePrice ? 'block' : 'none';
}


function setupButtonHandlers(sku) {
    document.getElementById('buyButton').addEventListener('click', function () {
        confirmPurchase(sku, false);
    });

    document.getElementById('offerPriceButton').addEventListener('click', function () {
        confirmPurchase(sku, true);
    });
}




function populateFormFieldsDetail(item) {
    const formElements = {
        name: 'name',
        price: 'price',
        description: 'description',
        stockAvailable: 'stockAvailable',
        productionCost: 'productionCost',
        offerablePrice: 'offerablePrice',
        itemListed: 'itemListed',
        /*sellerContact: 'sellerContact'*/
    };

    for (const [key, id] of Object.entries(formElements)) {
        const element = document.getElementById(id);
        if (element) {
            if (element.type === 'checkbox') {
                element.checked = item[key] || false;
            } else {
                element.value = item[key] || '';
            }
        }
    }
   
}


function isNullOrEmpty(value) {
    return value === null || value === undefined || value.trim() === '';
}

function confirmPurchase(sku, offer) {
    const quantity = parseInt(document.getElementById('quantity').value, 10);
    const itemPrice = parseFloat(document.getElementById('itemPrice').textContent.replace(/[^0-9.]/g, ''));
    const offerPrice = offer ? parseFloat(document.getElementById('offerPrice').value) || 0 : itemPrice;
    const totalPrice = offerPrice * quantity;

    const confirmationMessage = offer ?
        `You are making an offer for ${quantity} item(s) at a price of $${offerPrice.toFixed(2)} each.\nTotal Price: $${totalPrice.toFixed(2)}\nDo you want to proceed?` :
        `You are buying ${quantity} item(s) at a price of $${itemPrice.toFixed(2)} each.\nTotal Price: $${totalPrice.toFixed(2)}\nDo you want to proceed?`;

    if (confirm(confirmationMessage)) {
        console.log('Buyer confirmed to buy.');
        buyItem(sku, offer, offerPrice, quantity);
    } else {
        console.log('Purchase cancelled.');
    }
}
   //fetch call the buy or offer item
function buyItem(sku, offer, offerPrice, quantity) {
    const idToken = sessionStorage.getItem('idToken');
    const accessToken = sessionStorage.getItem('accessToken');
    const username = sessionStorage.getItem('username');
    const sellPrice = parseFloat(document.getElementById('itemPrice').textContent.replace(/[^0-9.]/g, ''));

    const receipt = {
        SellPrice: sellPrice,
        OfferPrice: offer ? offerPrice : 0,
        Profit: 0,
        Revenue: 0,
        Quantity: quantity,
        PendingSale: offer,
        BuyerHash: '',
        SKU: sku
    };

    fetch(baseUrl + '/api/ItemBuying/CreateASaleReceipt', {
        method: 'POST',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'userName': username,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(receipt)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to create sale receipt: ' + response.statusText);
            }
            return response.json();
        })
        .then(data => {
            console.log(data.Message);
            alert('Purchase successful! Detail email with the seller contact is sent to both seller and buyer.');
            //fetchItemInDetails(sku); // Refresh item details after the purchase
        })
        .catch(error => {
            console.error('Error:', error);
        });
}



document.querySelectorAll('.thumbnail').forEach(item => {
    item.addEventListener('click', event => {
        const mainImage = document.getElementById('mainImage');
        mainImage.src = item.src; // Change the main image source to the thumbnail source
        mainImage.alt = item.alt; // Update alt text as well
    });
});

