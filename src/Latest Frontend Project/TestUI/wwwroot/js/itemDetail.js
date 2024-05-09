
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

function setupButtonHandlers(sku) {
    document.getElementById('buyButton').addEventListener('click', function () {
        confirmPurchase(sku);
        
    });

    document.getElementById('offerPriceButton').addEventListener('click', function () {
        offerPrice(sku);
    });
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
            processItemDetails(data);
            updatePageContent(data);
        })
        .catch(error => {
            console.error('Error fetching item details:', error);
        });
}

function processItemDetails(item) {
    console.log('Item Name:', item.name);
    console.log('Item SKU:', item.sku);

    const offerablePrice = item.offerablePrice;  
    console.log('Offerable Price:', offerablePrice);

    
    const offerContainer = document.getElementById('itemOfferContainer');

    
    if (!offerablePrice) // If offerablePrice is false, hide the container
    {  
        offerContainer.style.display = 'none';
    } else
    {  
        offerContainer.style.display = 'block';
    }
}

function updatePageContent(item) {

    document.getElementById('itemName').textContent = item.name;
    document.getElementById('itemSKU').textContent = `SKU: ${item.sku}`;
    document.getElementById('itemPrice').textContent = `Price: $${item.price}`;
    document.getElementById('itemDescription').textContent = item.description;
    document.getElementById('itemStock').textContent = `Stock Available: ${item.stockAvailable}`;
 

    // Update the main image
    const mainImage = document.getElementById('mainImage');
    
    if (!item.imageUrls || isNullOrEmpty(item.imageUrls[0])) {
        mainImage.src = 'css/default_image.jpg';
    } else {
        mainImage.src = item.imageUrls[0];
    }

    const numberOfImages = item.imageUrls.length;
    console.log('number of image:', numberOfImages);
    

    for (let i = 1; i <= numberOfImages; i++) {
        const thumbnail = document.getElementById(`Image${i}`);
        if (thumbnail) {
            if (i <= numberOfImages && item.imageUrls[i - 1]) {
                const url = item.imageUrls[i - 1];
                thumbnail.src = url;
                thumbnail.alt = `Image ${i} of ${item.name}`;
                thumbnail.style.display = 'block'; // Show the thumbnail if there's a URL
                thumbnail.onclick = function () {
                    mainImage.src = thumbnail.src; // Update the main image on thumbnail click
                    mainImage.alt = thumbnail.alt;
                    thumbnail.style.display = 'block'
                };
            } else {
                thumbnail.style.display = 'none'; // Hide unused or empty thumbnails
            }
        }
    }

    // Update video thumbnails
    const numberOfVideos = item.videoUrls.length;
    console.log(numberOfVideos);
    if (numberOfVideos >= 1) {
        for (let i = 1; i <= numberOfVideos; i++) { // Assuming you have 2 video placeholders
            const videoThumbnail = document.getElementById(`Video${i}`);
            if (videoThumbnail) {
                const source = videoThumbnail.getElementsByTagName('source')[0];
                if (i <= numberOfVideos && item.videoUrls[i - 1]) {
                    source.src = item.videoUrls[i - 1];
                    videoThumbnail.load(); // Important to reload the video element to update the source
                    videoThumbnail.style.display = 'block'; // Show the video thumbnail
                    videoThumbnail.onclick = () => {
                        videoThumbnail.play(); // Auto-play the video
                    };
                } else {
                    videoThumbnail.style.display = 'none'; // Hide empty or unused video thumbnails
                }
            }
        }
    }

   
    
}
function isNullOrEmpty(value) {
    return value === null || value === undefined || value.trim() === '';
}

function confirmPurchase(sku) {
  
    const priceText = document.getElementById('itemPrice').textContent;
    const price = parseFloat(priceText.replace(/[^\d.]/g, "")); // Extract numeric value from text

  
    const quantity = parseInt(document.getElementById('quantity').value, 10);

    
    const totalPrice = price * quantity;

    // Create the confirmation message
    const confirmationMessage = `You are buying ${quantity} item(s) at a price of $${price.toFixed(2)} each.\nTotal Price: $${totalPrice.toFixed(2)}\nDo you want to proceed?`;

    
    var userConfirmed = confirm(confirmationMessage);

    if (userConfirmed) {
       
        console.log('Buyer confirm to buy.');
        buyItem(sku);  
    } else {
        console.log("Purchase cancelled.");
    }
}
function buyItem(sku) {

    const quantity = parseInt(document.getElementById('quantity').value, 10);  // Ensure you have an input with ID 'quantity'
    idToken = sessionStorage.getItem("idToken");
    accessToken = sessionStorage.getItem("accessToken");
    const username = sessionStorage.getItem('username');
    const sellPrice = parseFloat(document.getElementById('itemPrice').textContent.replace(/[^0-9\.]/g, ''));

    const receipt = {
        SellPrice: sellPrice,
        OfferPrice: 0,
        Profit: 0,
        Revenue: 0,
        Quantity: quantity,
        PendingSale: false,
        BuyerHash: "",
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
            fetchItemDetails(sku)
        })
        .then(data => {
            console.log(data.Message);
            alert('Purchase successful! Detail email with the seller contact is sent to both seller and buyer.');
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Failed to complete the purchase.');
        });
   
}

