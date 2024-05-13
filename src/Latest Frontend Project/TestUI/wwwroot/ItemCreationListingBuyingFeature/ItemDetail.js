
var baseUrl = 'https://themusicali.com:5000';
//var baseUrl = 'http://localhost:8080';



function loadDetail(skuNumber, option) {
    //const params = new URLSearchParams(window.location.search);
    const sku = skuNumber;
    const idToken = sessionStorage.getItem('idToken');
    const accessToken = sessionStorage.getItem('accessToken');
    const username = sessionStorage.getItem('username');
    if (option == 1) {
        document.querySelector('.buying_option').style.display = 'block';
    } else {
        document.querySelector('.buying_option').style.display = 'none'; // Ensure it's visible otherwise
    }
    setupNonSkuPageHandlers(); // Setup handlers that do not require the SKU

    if (!sku) {
        console.log('No SKU readed from the view. Showing item detail functionalities will be disabled.');
    } else {
        fetchItemDetails(sku); // Fetch details only if SKU is available
        if (!idToken || !accessToken || !username) {
            alert("Please login to access buying feature!!!");
            document.getElementById('buyButton').style.display = 'none';
            document.getElementById('offerPriceButton').style.display = 'none';
        } else {
            setupBuyOrOfferButtonHandlers(sku); // Setup button handlers that require the SKU
        }
        
    }
};

function setupNonSkuPageHandlers() {
    // Setup navigation links or buttons
    document.querySelectorAll('.nav-button').forEach(button => {
        button.addEventListener('click', function () {
            const section = document.querySelector(this.getAttribute('data-target'));
            if (section) {
                section.scrollIntoView({ behavior: 'smooth' });
            }
        });
    });

    // Initialize any tooltips on the page
    document.querySelectorAll('[data-tooltip]').forEach(element => {
        element.addEventListener('mouseenter', function () {
            const tooltipText = this.getAttribute('data-tooltip');
            const tooltip = document.createElement('div');
            tooltip.className = 'tooltip';
            tooltip.textContent = tooltipText;
            this.appendChild(tooltip);
            tooltip.style.left = this.offsetWidth / 2 - tooltip.offsetWidth / 2 + 'px'; // Center the tooltip
            tooltip.style.top = this.offsetTop - tooltip.offsetHeight - 10 + 'px'; // Position above the element
        });

        element.addEventListener('mouseleave', function () {
            this.removeChild(this.querySelector('.tooltip'));
        });
    });

    // Prepare modal windows
    document.querySelectorAll('.modal-trigger').forEach(trigger => {
        trigger.addEventListener('click', function () {
            const modalId = this.getAttribute('data-modal');
            const modal = document.getElementById(modalId);
            if (modal) {
                modal.style.display = 'block';
            }
        });
    });

    // Close modals when clicking on close buttons
    document.querySelectorAll('.modal-close').forEach(closeButton => {
        closeButton.addEventListener('click', function () {
            this.closest('.modal').style.display = 'none';
        });
    });

    // Maybe add more general event listeners or setups as needed
}

document.addEventListener('DOMContentLoaded', setupNonSkuPageHandlers);
//start adding code here

function setupBuyOrOfferButtonHandlers(sku) {
    document.getElementById('buyButton').addEventListener('click', function () {
        confirmPurchase(sku, false);
    });

    document.getElementById('offerPriceButton').addEventListener('click', function () {
        confirmPurchase(sku, true);
    });
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
            showOfferableItemDetails(data);
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

    // Update the main image
    const mainImage = document.getElementById('mainImage');
    mainImage.src = item.imageUrls[0] || 'path/to/default/image.jpg'; // Provide a default image if the main image URL is null
    
    // Update image thumbnails
    if (item.imageUrls.length > 1) {
        item.imageUrls.slice(0).forEach((url, index) => {
            const thumbnail = document.getElementById(`Image${index + 1}`);
            if (thumbnail) {
                if (url) {
                    thumbnail.src = url;
                    thumbnail.alt = `Image ${index + 1} of ${item.name}`;
                    thumbnail.style.display = ''; // Reset to default display style if hidden previously
                    thumbnail.addEventListener('click', function () {
                        mainImage.src = thumbnail.src; // Update main image on thumbnail click
                        mainImage.alt = thumbnail.alt;
                    });
                } else {
                    thumbnail.style.display = 'none'; // Hide the thumbnail if the URL is null
                }
            }
        });
    }

    // Update thumbnails for videos
    item.videoUrls.forEach((url, index) => {
        const videoThumbnail = document.getElementById(`Video${index + 1}`);
        if (videoThumbnail) {
            if (url) {
                if (videoThumbnail.children.length > 0) {
                    videoThumbnail.children[0].src = url; // Assuming the first child is the <source> element
                    videoThumbnail.load(); // Reload the video element to update the source
                    videoThumbnail.style.display = ''; // Reset to default display style if hidden previously
                    videoThumbnail.onclick = () => {
                        mainImage.style.display = 'none'; // Hide the main image
                        videoThumbnail.style.display = 'block'; // Display the video
                        videoThumbnail.play(); // Auto-play the video
                    };
                }
            } else {
                videoThumbnail.style.display = 'none'; // Hide the video thumbnail if the URL is null
            }
        }
    });
}

function showOfferableItemDetails(item) {
    const offerablePrice = item.offerablePrice;
    const offerContainer = document.getElementById('itemOfferContainer');

    offerContainer.style.display = offerablePrice ? 'block' : 'none';
}







function populateFormFieldsDetail(item) {
    const formElements = {
        name: 'name',
        price: 'price',
        description: 'description',
        stockAvailable: 'stockAvailable',
        productionCost: 'productionCost',
        offerablePrice: 'offerablePrice',
        itemListed: 'itemListed'
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
    const stock = parseInt(document.getElementById('itemStock').textContent.replace(/[^0-9.]/g, ''));
    const itemPrice = parseFloat(document.getElementById('itemPrice').textContent.replace(/[^0-9.]/g, ''));
    const offerPrice = offer ? parseFloat(document.getElementById('offerPrice').value) || 0 : itemPrice;
    const totalPrice = offerPrice * quantity;

    const confirmationMessage = offer ?
        `You are making an offer for ${quantity} item(s) at a price of $${offerPrice.toFixed(2)} each.\nTotal Price: $${totalPrice.toFixed(2)}\nDo you want to proceed?` :
        `You are buying ${quantity} item(s) at a price of $${itemPrice.toFixed(2)} each.\nTotal Price: $${totalPrice.toFixed(2)}\nDo you want to proceed?`;

    if (confirm(confirmationMessage)) {
        if (stock >= quantity && stock!=0) {
            console.log("stock: " + stock + " and quantity :" + quantity);
            console.log('Buyer confirmed to buy.');
            buyItem(sku, offer, offerPrice, quantity);
        }
        else {
            console.log('Item runs out of stock.');
            console.log("stock: " + stock + " and quantity :" + quantity);
            alert('Item stock is 0 or lower than the quantity you want. Cannot finish the sale!!!');
            fetchItemDetails(sku);
        }
        
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
            console.log('Sale is done');
            alert('Purchase successful! Detail email with the seller contact is sent to both seller and buyer.');
            fetchItemDetails(sku); // Refresh item details after the purchase
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

