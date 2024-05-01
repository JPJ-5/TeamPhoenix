



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
    });
});

var baseUrl = 'http://localhost:8080';
document.addEventListener('DOMContentLoaded', function () {
    

    document.getElementById('buyerHistoryBtn').addEventListener('click', function () {
        console.log('Buyer History clicked');
        // Additional functionality here
    });

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



document.getElementById('itemCreationBtn').addEventListener('click', function () {
    // Show the form container
    var formContainer = document.getElementById('itemCreationForm');
    formContainer.style.display = 'block';
});

document.getElementById('creationForm').addEventListener('submit', function (event) {
    event.preventDefault(); // Prevent the traditional form submission

    // Collect data from the form
    var formData = new FormData(this); // This captures all form inputs automatically

    // Example of handling the form data
    console.log('Submitting item creation data:', Object.fromEntries(formData.entries()));

    // Optionally, send the form data using AJAX
    // fetch('your-endpoint', {
    //     method: 'POST',
    //     body: formData
    // })
    // .then(response => response.json())
    // .then(data => console.log(data))
    // .catch(error => console.error('Error:', error));
});


document.getElementById('image').addEventListener('change', function () {
    validateFiles(this.files, 5, 7 * 1024 * 1024, ['image/jpeg', 'image/png', 'image/gif', 'image/tiff'], 'imageError');
});

document.getElementById('video').addEventListener('change', function () {
    validateFiles(this.files, 2, 500 * 1024 * 1024, ['video/mp4', 'video/quicktime'], 'videoError');
});

function validateFiles(files, maxFiles, maxSize, validTypes, errorElementId) {
    let errorMessages = [];
    if (files.length > maxFiles) {
        errorMessages.push(`You can only upload up to ${maxFiles} files.`);
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
        errorDiv.textContent = ''; // Clear error message when valid
    }
}

//document.getElementById('creationForm').addEventListener('submit', function (event) {
//    event.preventDefault();  // Prevent the form from submitting until validation is complete
    
//    if (errors.length > 0) {
//        document.getElementById('formErrors').innerHTML = errors.join("<br>");
//    }
//    else
//    {
//        document.getElementById('formErrors').innerHTML = '';
    
//        const username = sessionStorage.getItem('username');
//        if (!username)
//        {
//            alert("Username is not available. Please login again.");
//            return;
//        }

//        // FormData to hold the files and any other form data
//        const formData = new FormData(this);
//        formData.append('username', username); // Append username if needed to FormData directly
//        var userProfileUrl = `${baseUrl}/api/UploadFilesToSandBox/${username}`;
//        try
//        {
            
            //const response = await fetch(userProfileUrl,
            //{
            //    method: 'POST',
            //    headers: {
            //        'Authentication': idToken,
            //        'Authorization': accessToken,
            //        'userName': username
            //    },
            //    body: formData, // FormData will be sent as 'multipart/form-data'
            //});

//            if (!response.ok)
//            {
//                throw new Error('Failed to upload files.');
//            }

//            const result = await response.json();
//            alert('Files uploaded successfully.');
//            console.log(result); // Process the response further if needed
//        }
//        catch (error)
//        {
//            console.error('Error:', error);
//            alert('Error uploading files: ' + error.message);
//        }
//    }


//});


//document.getElementById('creationForm').addEventListener('submit', async function (event) {
//    event.preventDefault(); // Prevent the default form submission

//    // FormData to hold the file data
//    var formData = new FormData();
//    var files = document.querySelector('[name="image[]"]').files;
//    Array.from(files).forEach(file => {
//        formData.append('files', file);  // Ensure the backend expects 'files' as the key
//    });
//    var videos = document.querySelector('[name="video[]"]').files;
//    Array.from(videos).forEach(video => {
//        formData.append('files', video);
//    });
//    const username = sessionStorage.getItem('username');
//    const idToken = sessionStorage.getItem("idToken");
//    const accessToken = sessionStorage.getItem("accessToken");

//    try {
//        // First Fetch Call to upload files
//        var sandBoxPath = `${baseUrl}/api/UploadS3/UploadFilesToSandBox`;

//        let fileResponse = await fetch(sandBoxPath, {
//            method: 'POST',
//            headers: {
//                'Authentication': idToken,
//                'Authorization': accessToken,
//                'userName': username
//            },
//            body: formData
//        });

//        if (!fileResponse.ok) {
//            let text = await fileResponse.text();  // Read response as text first to avoid JSON parse error
//            throw new Error(`Error uploading files: ${text}`);
//        }

//        try {
//            let fileData = await fileResponse.json();
//            console.log('Files uploaded:', fileData);
//        } catch (e) {
//            // If an error occurs during JSON parsing, log the error and the response
//            console.error('Could not parse response as JSON', e);
//            let text = await fileResponse.text();
//            console.log('Response:', text);
//        }



//        var sandBoxPath = `${baseUrl}/api/UploadS3/UpLoadFolderToS3`;

//        let fileResponse = await fetch(sandBoxPath, {
//            method: 'POST',
//            headers: {
//                'Authentication': idToken,
//                'Authorization': accessToken,
//                'userName': username
//            },
//            body: formData
//        });

//        if (!response.ok) {
//            throw new Error('Network response was not ok.');
//        }

//        // Assuming the server responds with JSON data
//        let data = await response.json();
//        console.log('Files uploaded:', data);
//        return data;
//    } catch (error) {
//        console.error('Error during file upload to S3:', error);
//    }





//        //// Prepare the second FormData for the remaining data
//        //var itemFormData = new FormData();
//        //itemFormData.append('name', document.getElementById('name').value);
//        //itemFormData.append('price', document.getElementById('price').value);
//        //itemFormData.append('description', document.getElementById('description').value);
//        //itemFormData.append('stockAvailable', document.getElementById('stockAvailable').value);
//        //itemFormData.append('productionCost', document.getElementById('productionCost').value);
//        //itemFormData.append('offerablePrice', document.getElementById('offerablePrice').checked);
//        //itemFormData.append('sellerContact', document.getElementById('sellerContact').value);

//        //// Second Fetch Call to send the rest of the data
//        //let createResponse = await fetch('http://your-ec2-ip/CreateItemAPI', {
//        //    method: 'POST',
//        //    headers: {
//        //        'Authorization': `Bearer ${accessToken}`,
//        //        'userName': username
//        //    },
//        //    body: itemFormData
//        //});
//        //if (!createResponse.ok) {
//        //    let text = await createResponse.text();
//        //    throw new Error(`Error creating item: ${text}`);
//        //}
//        //let createData = await createResponse.json();
//        //console.log('Item created:', createData);
//        //// Display success message or perform further actions

//    //} catch (error) {
//    //    console.error('Error:', error);
//    //    // Handle errors here (e.g., display an error message)
//    //}
//});
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
            let text = await uploadResponse.text();
            throw new Error(`Error uploading files to sandbox: ${text}`);
        }

        // Assuming the server responds with JSON data
        let uploadData = await uploadResponse.json();
        console.log('Files uploaded to sandbox:', uploadData);

        //// Second Fetch Call to upload files from sandbox to S3
        //var s3Path = `${baseUrl}/api/UploadS3/UpLoadFolderToS3`;

        //let s3Response = await fetch(s3Path, {
        //    method: 'POST',
        //    headers: {
        //        'Authentication': idToken,
        //        'Authorization': accessToken,
        //        'userName': username
        //    },
        //});

        //if (!s3Response.ok) {
        //    let text = await s3Response.text();

        //    throw new Error(`Error during file upload to S3: ${text}`);
        //}

        //// Assuming the server responds with JSON data
        //let s3Data = await s3Response.json();

        //console.log('Files uploaded to S3:', s3Data);
        //console.log("Message: ", s3Data.Message);
        //console.log("URLs: ", s3Data.Urls);
        //console.log("SKU: ", s3Data.Sku);

        //let sku = "string";
        //let mediaFile = s3Data.Urls;
        //let userhash = "string";
        let images = "string";
        let videos = "string";
        // 3rd Fetch Call to item creation 
        var itemCreatePath = `${baseUrl}/api/ItemCreation/CreateAnItem`;

        const name = formData.get('name');
        const price = formData.get('price');
        const description = formData.get('description');
        const stock = formData.get('stockAvailable');
        const cost = formData.get('productionCost');
        const offerable = formData.get('offerablePrice');
        const sellerContact = formData.get('sellerContact');
        const itemListed = formData.get('itemListed');
        
        const data = { name, price, description, stock, cost, offerable, sellerContact, images, videos, itemListed };

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
            let text = await itemCreationResponse.text();
            throw new Error(`Error during item creation: ${text}`);
        }



    } catch (error) {
        console.error('Error:', error);
    }
});








