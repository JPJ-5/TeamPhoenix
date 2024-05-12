
var baseUrl = 'http://localhost:8080';
document.getElementById('itemCreationBtn').addEventListener('click', function () {

    hideAllSectionsFromCraft();
    var formContainer = document.getElementById('itemCreationForm'); // Show the form container
    const idToken = sessionStorage.getItem('idToken');
    const accessToken = sessionStorage.getItem('accessToken');
    const username = sessionStorage.getItem('username');
   
    if (!idToken || !accessToken || !username) {
        alert("Please login to use this feature!!!");
    }
    else {
        formContainer.style.display = 'block';
    }

});

function hideAllSectionsFromCraft() {
    document.getElementById('itemCreationForm').style.display = 'none';
    document.getElementById('itemModificationContainer').style.display = 'none';
    document.getElementById('itemModificationForm').style.display = 'none';
    document.getElementById('pendingSaleContainer').style.display = 'none';
    document.getElementById('financialProgressReportView').style.display = 'none';
    document.getElementById('itemsListingContainer').style.display = 'none';
    document.getElementById('priceRangeSortingView').style.display = 'none';
    if (document.getElementById('itemDetailsContainer')) {
        document.getElementById('itemDetailsContainer').style.display = 'none';
    }
}

document.getElementById('creationForm').addEventListener('submit', function (event) {
    event.preventDefault(); // Prevent the traditional form submission

    // Collect data from the form
    var formData = new FormData(this); // This captures all form inputs automatically

    console.log('Submitting item creation data:', Object.fromEntries(formData.entries()));

});


document.getElementById('image').addEventListener('change', function () {
    validateFiles(this.files, 5, 7 * 1024 * 1024, ['image/jpeg', 'image/png', 'image/gif', 'image/tiff'], 'imageError');
});

document.getElementById('video').addEventListener('change', function () {
    validateFiles(this.files, 2, 500 * 1024 * 1024, ['video/mp4', 'video/quicktime'], 'videoError');
});

function validateFiles(files, maxNumberOfFiles, maxSize, validTypes, errorElementId) {
    let errorMessages = [];
    if (files.length > maxNumberOfFiles) {
        errorMessages.push(`You can only upload up to ${maxNumberOfFiles} files.`);
    }

    for (let i = 0; i < files.length; i++) {
        if (!validTypes.includes(files[i].type)) {
            errorMessages.push(`Invalid file type: ${files[i].name}`);
        }
        if (files[i].size > maxSize) {
            errorMessages.push(`${files[i].name} is too large. Maximum size is ${maxSize / 1024 / 1024} MB.`);
        }
    }

    let errorContainer = document.getElementById(errorElementId);
    if (errorMessages.length > 0) {
        errorContainer.innerHTML = errorMessages.join('<br>');
        errorContainer.style.color = 'red';
    } else {
        errorContainer.innerHTML = '';
    }
}

function validateDescription() {
    var description = document.getElementById('description');
    var errorDiv = document.getElementById('descriptionError');

    // Regex that matches allowed characters
    if (!/^[a-zA-Z0-9 ,:"'()!@#$%&*]*$/.test(description.value)) {
        errorDiv.textContent = "Description can only include alphanumeric characters and ,: '\"()!@#$%&*.";
        description.value = description.value.replace(/[^a-zA-Z0-9 ,:"'()!@#$%&*]/g, '');
    } else {
        errorDiv.textContent = ''; 
    }
}


document.getElementById('creationForm').addEventListener('submit', async function (event) {
    event.preventDefault(); // Prevent the default form submission
    const username = sessionStorage.getItem('username');
    const idToken = sessionStorage.getItem("idToken");
    const accessToken = sessionStorage.getItem("accessToken");
    // FormData to hold the file data for the first call
    var formData = new FormData(event.target);
    var images = document.querySelector('[name="image[]"]').files;
    Array.from(images).forEach(file => {
        formData.append('files', file);
    });
    var videos = document.querySelector('[name="video[]"]').files;
    Array.from(videos).forEach(video => {
        formData.append('files', video);
    });


    try {
        // First Fetch Call to upload files
        var uploadPath = `${baseUrl}/api/UploadS3/UploadFilesToSandBox`;

        let uploadResponse = await fetch(uploadPath, {
            method: 'POST',
            headers: {
                'Authentication': idToken,
                'Authorization': accessToken,
                'userName': username
            },
            body: formData
        });

        if (!uploadResponse.ok) {
            alert('there is an error while uploading your items pictures and videos. Please try again!')
            let text = await uploadResponse.text();
            throw new Error(`Error uploading files to sandbox: ${text}`);
        }

        // Assuming the server responds with JSON data
        let uploadData = await uploadResponse.json();
        console.log('Files uploaded to sandbox:', uploadData);


        let images = "string";
        let videos = "string";
        // 2nd Fetch Call to item creation 
        var itemCreatePath = `${baseUrl}/api/ItemCreation/CreateAnItem`;

        const name = formData.get('name');
        const price = formData.get('price');
        const description = formData.get('description');
        const stockAvailable = formData.get('stockAvailable');
        const productionCost = formData.get('productionCost');
        const offerablePrice = Boolean(formData.get('offerablePrice'));
        const sellerContact = formData.get('sellerContact');
        const listed = Boolean(formData.get('itemListed'));

        const data = { name, price, description, stockAvailable, productionCost, offerablePrice, sellerContact, images, videos, listed };

        let itemCreationResponse = await fetch(itemCreatePath, {
            method: 'POST',
            headers: {
                'Authentication': idToken,
                'Authorization': accessToken,
                'userName': username,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        if (!itemCreationResponse.ok) {
            alert('Error occured while adding item to database. Please try again!')
            let text = await itemCreationResponse.text();
            throw new Error(`Error during item creation: ${text}`);
        }
        else {
            alert('Your item is sucessfully added to CraftVerify, yade!')
        }



    } catch (error) {
        console.error('Error:', error);
    }
});