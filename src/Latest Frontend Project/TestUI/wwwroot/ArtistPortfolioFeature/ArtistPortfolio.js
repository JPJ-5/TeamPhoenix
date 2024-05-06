
var baseUrl = 'https://themusicali.com:5000';
//var baseUrl = 'http://localhost:8080';

document.addEventListener('DOMContentLoaded', function () {

    var activeUsername = sessionStorage.getItem('username');
    // Reset page to default to remove media when Upload/Deletes occur
    resetArtistPortfolioView();
    loadProfileData(activeUsername);
});

function reload() {
    var activeUsername = sessionStorage.getItem('username');
    // Reset page to default to remove media when Upload/Deletes occur
    resetArtistPortfolioView();
    loadProfileData(activeUsername);
}

//Calls LoadApi from Controller to load all ArtistProfile data
function loadProfileData(username) {
    var feedbackBox = document.getElementById('portfolio-feedback');
    idToken = sessionStorage.getItem("idToken");
    accessToken = sessionStorage.getItem("accessToken");
    
    fetch(`${baseUrl}/ArtistPortfolio/api/loadApi`, {
        method: 'GET',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'Content-Type': 'application/json',
            'Username': username
        },
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                return response.text().then(text => { throw new Error(text); });
            }
        })
        .then(profileData => {
            displayArtistProfile(profileData);
        })
        .catch(error => {
            console.error('Error:', error);
            feedbackBox.textContent = 'Error loading artist profile. Please try again.';
            feedbackBox.style.color = 'red';
        });
}

//functions that plays media by the slot number for both mp3 and mp4
function playMedia(slot) {
    const mediaSlot = document.getElementById(`slot-${slot}`);
    const media = mediaSlot.querySelector('audio') || mediaSlot.querySelector('video');
    if (media) {
        media.play();
    }
}

function displayArtistProfile(portfolioInfo) {
    const infoSlot = document.getElementById(`info-upload`);
    var activeUsername = sessionStorage.getItem('username');
    const usernameParagraph = document.createElement('h1');
    usernameParagraph.innerHTML = `<strong>${activeUsername}</strong>`;
    infoSlot.appendChild(usernameParagraph);

    const visibilityLabel = document.createElement('label');
    visibilityLabel.innerHTML = 'Visibility:';

    // Create a True button
    const trueButton = document.createElement('button');
    trueButton.textContent = 'True';
    trueButton.style.backgroundColor = portfolioInfo[`visibility`] ? 'green' : ''; // Highlight if visibility is true
    trueButton.disabled = portfolioInfo[`visibility`]; // Disable if visibility is true
    trueButton.addEventListener('click', function () {
        // Call updateVis function with the new visibility value (true)
        updateVis(true);
    });

    // Create a False button
    const falseButton = document.createElement('button');
    falseButton.textContent = 'False';
    falseButton.style.backgroundColor = !portfolioInfo[`visibility`] ? 'red' : ''; // Highlight if visibility is false
    falseButton.disabled = !portfolioInfo[`visibility`]; // Disable if visibility is false
    falseButton.addEventListener('click', function () {
        // Call updateVis function with the new visibility value (false)
        updateVis(false);
    });

    // Append the buttons to the label
    visibilityLabel.appendChild(trueButton);
    visibilityLabel.appendChild(falseButton);

    // Append the label to the info slot
    infoSlot.appendChild(visibilityLabel);

    //Display for other occupation with selection boxes
    if (portfolioInfo[`occupation`]){
        const info = portfolioInfo[`occupation`];
    
        //paragraphs for genre and description if existing
        const infoParagraph = document.createElement('p');
        infoParagraph.innerHTML = `<strong>Occupation: </strong>${info}`;
        infoSlot.appendChild(infoParagraph);

        //delete button for existing picture
        const deleteButton = document.createElement('button');
        deleteButton.textContent = 'Delete info';
        deleteButton.addEventListener('click', function() {
            deleteInfo(`occupation`);
        });
        infoSlot.appendChild(deleteButton);

    } else {
        //Creating select dropdown for occupation when null
        const options = ['Composer', 'Arranger', 'Producer', 'Instrumentalist', 'None'];

        const infoParagraph = document.createElement('p');
        infoParagraph.innerHTML = `<strong>Occupation: </strong>No occupation yet`;
        infoSlot.appendChild(infoParagraph);

        //adding all the options into the dropwown selection
        const dropdown = document.createElement('select');
        dropdown.id = `occupation-input`;
        options.forEach(optionText => {
            const option = document.createElement('option');
            option.textContent = optionText;
            dropdown.appendChild(option);
        });
        infoParagraph.appendChild(dropdown);

        //creating upload button for user to activate upload
        const uploadButton = document.createElement('button');
        uploadButton.textContent = `Upload Occupation`;
        uploadButton.addEventListener('click', function() {
            triggerInfoInput(`occupation`);
        });
        infoSlot.appendChild(uploadButton);
    }

    //creating display for bio and location ifo
    var artistInfo = ["bio", "location"];
    for (let i = 0; i < 2; i ++) {
        if (!portfolioInfo[`${artistInfo[i]}`]) {

            const infoParagraph = document.createElement('p');
            infoParagraph.innerHTML = `<strong>${artistInfo[i]}: </strong>No ${artistInfo[i]} yet`;
            infoSlot.appendChild(infoParagraph);
            
            //input field created for info that it missing to be uplaoded when upload button is clicked
            const portfolioInput = document.createElement('input');
            portfolioInput.type = 'text';
            portfolioInput.id = `${artistInfo[i]}-input`;
            portfolioInput.placeholder = `Enter your desired ${artistInfo[i]}`;
            infoSlot.appendChild(portfolioInput);
            
            const uploadButton = document.createElement('button');
            uploadButton.textContent = `Upload ${artistInfo[i]}`;
            uploadButton.addEventListener('click', function() {
                triggerInfoInput(`${artistInfo[i]}`);
            });
            infoSlot.appendChild(uploadButton);

        } else {
            const info = portfolioInfo[`${artistInfo[i]}`];
        
            const infoParagraph = document.createElement('p');
            infoParagraph.innerHTML = `<strong>${artistInfo[i]}: </strong>${info}`;
            infoSlot.appendChild(infoParagraph);

            const deleteButton = document.createElement('button');
            deleteButton.textContent = 'Delete info';
            deleteButton.addEventListener('click', function() {
                deleteInfo(`${artistInfo[i]}`);
            });
            infoSlot.appendChild(deleteButton);
        }
    }
    
    //Creating display for the profile picture
    if (portfolioInfo[`file0`]) {
        const mediaSlot = document.getElementById(`file-upload-0`);

        //creating image using extension from backend to create right type
        const image = document.createElement('img');
        var ext = portfolioInfo[`file${0}Ext`];
        const filePath = 'data:image/' + ext + ';base64,' + portfolioInfo[`file0`];
        image.src = filePath;
        mediaSlot.appendChild(image);

        const deleteButton = document.createElement('button');
        deleteButton.textContent = 'Delete Profile Pic';
        deleteButton.addEventListener('click', function() {
            deleteFile(0);
        });
        mediaSlot.appendChild(deleteButton);
    } else {
        const uploadButton = document.createElement('button');
        const mediaSlot = document.getElementById(`file-upload-0`);
        uploadButton.textContent = 'Upload Profile Pic';
        uploadButton.addEventListener('click', function() {
            triggerFileInput(0);
        });
        mediaSlot.appendChild(uploadButton);
    }

    // Creating display for media sources based on file type for each media slot, Genre and Description included
    for (let i = 1; i <= 5; i++) {
        const fileContent = portfolioInfo[`file${i}`];
        document.getElementById('slotsTable').style.display = 'block';
        var slotsTable = document.getElementById('slotsTable');

        var row = slotsTable.insertRow();
        var nameCell = row.insertCell();
        var genreCell = row.insertCell();
        var descCell = row.insertCell();
        var mediaCell = row.insertCell();
        var deleteCell = row.insertCell();

        if (fileContent == "") {
            // If null then we load upload button and input for info and media and set genre and desc to display lack of media
            nameCell.innerHTML = 'No File Yet';
            genreCell.innerHTML = 'N/A';
            descCell.innerHTML = 'N/A';

            const uploadButton = document.createElement('button');
            uploadButton.textContent = 'Upload Media';
            uploadButton.addEventListener('click', function () {
                triggerFileInput(i);
            });
            const fileInput = document.createElement('input');
            fileInput.type = 'file';
            fileInput.id = `media-slot-${i}-content`;
            fileInput.style.display = 'none';
            deleteCell.appendChild(fileInput);
            deleteCell.appendChild(uploadButton);

            // Input boxes for Genre and Desc input if null
            const genreInput = document.createElement('input');
            genreInput.type = 'text';
            genreInput.id = `genre-input-${i}`;
            genreInput.placeholder = 'Enter Genre';
            genreCell.appendChild(genreInput);

            const descInput = document.createElement('input');
            descInput.type = 'text';
            descInput.id = `desc-input-${i}`;
            descInput.placeholder = 'Enter Description';
            descCell.appendChild(descInput);

        } else {
            // Create display for Genre and Desc paragraphs when not null
            const name = portfolioInfo[`file${i}Name`] || 'N/A';
            const genre = portfolioInfo[`file${i}Genre`] || 'N/A';
            const desc = portfolioInfo[`file${i}Desc`] || 'N/A';

            nameCell.innerHTML = name;
            genreCell.innerHTML = genre;
            descCell.innerHTML = desc;

            // Get extension from backend data to create correct media type for base64 strings
            const extension = portfolioInfo[`file${i}Ext`];
            const supportedAudioFormats = ['.mp3', '.wav'];
            const supportedVideoFormats = ['.mp4'];

            // Check for correct extension and for which exact type to construct media
            if (supportedAudioFormats.includes(extension) || supportedVideoFormats.includes(extension)) {
                const media = extension === '.mp3' ? document.createElement('audio') : document.createElement('video');
                media.src = 'data:audio/' + extension.substring(1) + ';base64,' + fileContent;
                media.controls = true;
                media.style.display = 'block';
                mediaCell.appendChild(media);

                // Delete button if user desires to remove this media
                const deleteButton = document.createElement('button');
                deleteButton.textContent = 'Delete File';
                deleteButton.addEventListener('click', function () {
                    deleteFile(i);
                });

                deleteCell.appendChild(deleteButton);
            }
        }
    }
}

//function to call api to delete file along with genre and description
function deleteFile(slot){
    var activeUsername = sessionStorage.getItem('username');
    idToken = sessionStorage.getItem("idToken"); 
    accessToken = sessionStorage.getItem("accessToken");
    // Send POST request to delete API endpoint
    deleteForm = new FormData();
    deleteForm.append('Username', activeUsername);
    deleteForm.append('SlotNumber', slot);
    fetch(`${baseUrl}/ArtistPortfolio/api/deleteApi`, {
        method: 'POST',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken
        },
        body: deleteForm // Remove 'Content-Type' header and send FormData directly
    })
    .then(response => {
        if (response.ok) {
            return response.text();
        } else {
            return response.text().then(text => { throw new Error(text); });
        }
    })
    .then(responseText => {
        console.log(responseText); // Log success message
        reload(); //reloads page when info is changed //reload page once data is changed in ayway
    })
    .catch(error => {
        console.error('Error:', error); // Log error message
    });
}

//function to call api to delete section in profile info
function deleteInfo(section) {
    var activeUsername = sessionStorage.getItem('username');
    const deleteSection = [activeUsername, `${section}`];
    idToken = sessionStorage.getItem("idToken");
    accessToken = sessionStorage.getItem("accessToken");

    // Send POST request to delete info API endpoint
    fetch(`${baseUrl}/ArtistPortfolio/api/delInfoApi`, {
        method: 'POST',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(deleteSection) 
    })
    .then(response => {
        if (response.ok) {
            return response.text();
        } else {
            return response.text().then(text => { throw new Error(text); });
        }
    })
    .then(responseText => {
        console.log(responseText); // Log success message
        reload(); //reloads page when info is changed // reloads page when info is change
    })
    .catch(error => {
        console.error('Error:', error); // Log error message
    });
}

//function to trigger file upload from user including genre and description boxes if used
function triggerFileInput(slot) {
    //checking for inputted media in the input box created for it
    const input = document.getElementById(`media-slot-${slot}-content`);
    idToken = sessionStorage.getItem("idToken");
    accessToken = sessionStorage.getItem("accessToken");
    input.addEventListener('change', function() {
        const file = input.files[0];

        // Validate the file before uploading
        const validation = validateFile(file);
        if (!validation.valid) {
            console.error(validation.message);
            return;
        }

        // Create FormData object to send file and any genre or desc if used
        const formData = new FormData();
        formData.append('username', sessionStorage.getItem('username'));
        formData.append('slot', slot);
        formData.append('file', file);

        //only check if 1-5 media and only get genre and desc if not null
        if (slot > 0) {
            const genre = document.getElementById(`genre-input-${slot}`).value; // Use slot number to identify genre input
            const description = document.getElementById(`desc-input-${slot}`).value; // Use slot number to identify description input
            formData.append('genre', genre);
            formData.append('desc', description);
        } else {
            const genre = null 
            const description = null
        }

        // Send POST request to upload API endpoint
        fetch(`${baseUrl}/ArtistPortfolio/api/uploadApi`, {
            method: 'POST',
            headers: {
                'Authentication': idToken,
                'Authorization': accessToken
            },
            body: formData 
        })
        .then(response => {
            if (response.ok) {
                return response.text();
            } else {
                return response.text().then(text => { throw new Error(text); });
            }
        })
        .then(responseText => {
            console.log(responseText); // Log success message
            reload(); //reloads page when info is changed
        })
        .catch(error => {
            console.error('Error:', error); // Log error message
        });
    });
    input.click();
}

//function to trigger upload any Profile info(bio, location, occupation) takes in name of section being modified
function triggerInfoInput(section) {
    const input = document.getElementById(`${section}-input`);
    const inputValue = input.value;
    var activeUsername = sessionStorage.getItem('username');
    const sectionUpdate = [activeUsername, `${section}`, inputValue];
    idToken = sessionStorage.getItem("idToken");
    accessToken = sessionStorage.getItem("accessToken");

    // Send POST request to upload API endpoint
    fetch(`${baseUrl}/ArtistPortfolio/api/updateInfoApi`, {
        method: 'POST',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(sectionUpdate) 
    })
    .then(response => {
        if (response.ok) {
            return response.text();
        } else {
            return response.text().then(text => { throw new Error(text); });
        }
    })
    .then(responseText => {
        console.log(responseText); // Log success message
        reload(); //reloads page when info is changed //creloading page when data is changed
    })
    .catch(error => {
        console.error('Error:', error); // Log error message
    });
}

function resetArtistPortfolioView() {
    // Remove all media slot objects for reload
    const slotsTable = document.getElementById('slotsTable');
    while (slotsTable.rows.length > 1) {
        slotsTable.deleteRow(1);
    }

    // Remove profile pic for reload
    const profilePicSlot = document.getElementById('file-upload-0');
    profilePicSlot.innerHTML = '';

    const info = document.getElementById('info-upload');
    info.innerHTML = '';
}

function validateFile(file) {
    const ALLOWED_EXTENSIONS = new Set(['jpg', 'jpeg', 'png', 'gif', 'mp3', 'mp4', 'wav']);
    const MAX_FILE_SIZE_MB = 10; // Maximum file size in MB
    const fileNameParts = file.name.split('.');
    const fileExtension = fileNameParts[fileNameParts.length - 1].toLowerCase();

    if (!ALLOWED_EXTENSIONS.has(fileExtension)) {
        return { valid: false, message: 'Invalid file extension' };
    }

    if (file.size > MAX_FILE_SIZE_MB * 1024 * 1024) {
        return { valid: false, message: 'File size exceeds the limit' };
    }

    return { valid: true };
}

function updateVis(vis) {
    var activeUsername = sessionStorage.getItem('username');
    idToken = sessionStorage.getItem("idToken");
    accessToken = sessionStorage.getItem("accessToken");
    var payload = {
        username: activeUsername,
        visibility: vis
    }

    fetch(`${baseUrl}/ArtistPortfolio/api/updateVisibility`, {
        method: 'POST',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload),
    })
    .then(response => {
        if (response.ok) {
            return response.text();
        } else {
            return response.text().then(text => { throw new Error(text); });
        }
    })
    .then(responseText => {
        reload(); //reloads page when info is changed
    })
    .catch(error => {
        console.error('Error:', error); // Log error message
    });
}


//to be implemented later for users searcing you
//copy of normal display with all input and delete objects taken away
function displayOtherArtiste(username, portfolioInfo) {
    const infoSlot = document.getElementById(`info-upload`);
    const usernameParagraph = document.createElement('h1');
    usernameParagraph.innerHTML = `<strong>${username}</strong>`;
    infoSlot.appendChild(usernameParagraph);

    const infoParagraph = document.createElement('p');
    infoParagraph.innerHTML = `<strong>${portfolioInfo[`username`]}`;
    infoSlot.appendChild(infoParagraph);

    if (portfolioInfo[`occupation`]){
        const info = portfolioInfo[`occupation`];
    
        const infoParagraph = document.createElement('p');
        infoParagraph.innerHTML = `<strong>Occupation: </strong>${info}`;
        infoSlot.appendChild(infoParagraph);

    } else {
        const infoParagraph = document.createElement('p');
        infoParagraph.innerHTML = `<strong>Occupation: </strong>No occupation chosen`;
        infoSlot.appendChild(infoParagraph);
    }

    var artistInfo = ["bio", "location"];
    for (let i = 0; i < 2; i ++) {
        if (!portfolioInfo[`${artistInfo[i]}`]) {


            const infoParagraph = document.createElement('p');
            infoParagraph.innerHTML = `<strong>${artistInfo[i]}: </strong>No ${artistInfo[i]} yet`;

            infoSlot.appendChild(infoParagraph);
        } else {
            const info = portfolioInfo[`${artistInfo[i]}`];
        
            const infoParagraph = document.createElement('p');
            infoParagraph.innerHTML = `<strong>${artistInfo[i]}: </strong>${info}`;
            infoSlot.appendChild(infoParagraph);
        }
    }


    if (portfolioInfo[`file0`]) {
        const image = document.createElement('img');
        var ext = portfolioInfo[`file${0}Ext`];
        const filePath = 'data:image/' + ext + ';base64,' + portfolioInfo[`file0`];
        image.src = filePath;
        const deleteButton = document.createElement('button');
        deleteButton.textContent = 'Delete Profile Pic';
        deleteButton.addEventListener('click', function() {
            deleteFile(0);
        });
        const mediaSlot = document.getElementById(`file-upload-0`);
        mediaSlot.appendChild(image);
        mediaSlot.appendChild(deleteButton);
    } else {
        const picParagraph = document.createElement('p');
        picParagraph.innerHTML = `<strong> No picture yet`;
        const mediaSlot = document.getElementById(`file-upload-0`);
        mediaSlot.appendChild(picParagraph);
    }

    // Creating display for media sources based on file type for each media slot, Genre and Description included
    for (let i = 1; i <= 5; i++) {
        const fileContent = portfolioInfo[`file${i}`];
        document.getElementById('slotsTable').style.display = 'block';
        var slotsTable = document.getElementById('slotsTable');

        var row = slotsTable.insertRow();
        var nameCell = row.insertCell();
        var genreCell = row.insertCell();
        var descCell = row.insertCell();
        var mediaCell = row.insertCell();

        if (fileContent == "") {
            // If null then we load upload button and input for info and media and set genre and desc to display lack of media
            nameCell.innerHTML = 'No File Yet';
            genreCell.innerHTML = 'N/A';
            descCell.innerHTML = 'N/A';

        } else {
            // Create display for Genre and Desc paragraphs when not null
            const name = portfolioInfo[`file${i}Name`] || 'N/A';
            const genre = portfolioInfo[`file${i}Genre`] || 'N/A';
            const desc = portfolioInfo[`file${i}Desc`] || 'N/A';

            nameCell.innerHTML = name;
            genreCell.innerHTML = genre;
            descCell.innerHTML = desc;

            // Get extension from backend data to create correct media type for base64 strings
            const extension = portfolioInfo[`file${i}Ext`];
            const supportedAudioFormats = ['.mp3', '.wav'];
            const supportedVideoFormats = ['.mp4'];

            // Check for correct extension and for which exact type to construct media
            if (supportedAudioFormats.includes(extension) || supportedVideoFormats.includes(extension)) {
                const media = extension === '.mp3' ? document.createElement('audio') : document.createElement('video');
                media.src = 'data:audio/' + extension.substring(1) + ';base64,' + fileContent;
                media.controls = true;
                media.style.display = 'block';
                mediaCell.appendChild(media);

            }
        }
    }

}