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
    fetch(`http://localhost:8080/api/GetItemDetail?sku=${sku}`, { // Adjust the URL based on your actual API URL
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
    document.getElementById('itemName').textContent = item.name;
    document.getElementById('itemSKU').textContent = `SKU: ${item.sku}`;
    document.getElementById('itemPrice').textContent = `Price: $${item.price}`;
    document.getElementById('itemDescription').textContent = item.description;
    document.getElementById('itemStock').textContent = `Stock Available: ${item.stockAvailable}`;
    document.getElementById('itemSellerContact').textContent = item.sellerContact;


    // Images handling
    const mainImage = document.getElementById('mainImage');
    const thumbnails = document.querySelector('.thumbnail-container');
    thumbnails.innerHTML = ''; // Clear existing thumbnails

    item.imageUrls.slice(0, 5).forEach((url, index) => {
        if (index === 0) {
            mainImage.src = url; // Set the first image as the main image
            mainImage.alt = `Main Image for ${item.name}`;
        } else {
            const img = document.createElement('img');
            img.src = url;
            img.alt = `Thumbnail ${index}`;
            img.classList.add('thumbnail');
            img.onclick = () => {
                mainImage.src = url; // Update main image on thumbnail click
                mainImage.alt = `Main Image for ${item.name}`;
            };
            thumbnails.appendChild(img);
        }
    });

    // Video handling
    const videoContainer = document.querySelector('.video-container');
    videoContainer.innerHTML = ''; // Clear previous videos
    item.videoUrls.slice(0, 2).forEach(url => {
        const video = document.createElement('video');
        video.setAttribute('controls', true);
        video.innerHTML = `<source src="${url}" type="video/mp4">Your browser does not support the video tag.`;
        videoContainer.appendChild(video);
    });
}



