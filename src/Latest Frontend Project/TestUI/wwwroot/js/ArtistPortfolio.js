
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('enter-artistPortfolio').addEventListener('click', function () {
        // Reset page to default to remove media when Upload/Deletes occur
        resetArtistPortfolioView();
        loadProfileData();
    });
});


//Calls LoadApi from Controller to load all ArtistProfile data
function loadProfileData() {
    var activeUsername = sessionStorage.getItem('username');
    var profileDiv = document.getElementById('artistPortfolioView');
    var feedbackBox = document.getElementById('portfolio-feedback');
    
    fetch('http://localhost:8080/ArtistPortfolio/api/loadApi', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(activeUsername)
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
    console.log('Profile info:', portfolioInfo);

    // Write in Artist Info
    document.getElementById('bio-content').textContent = portfolioInfo.bio || 'N/A';
    document.getElementById('occupation-content').textContent = portfolioInfo.occupation || 'N/A';
    document.getElementById('location-content').textContent = portfolioInfo.location || 'N/A';

    // Track the number of elements that need to be loaded
    let elementsToLoad = 0;

    if (portfolioInfo[`file0`]) {
        elementsToLoad++;
        const image = document.createElement('img');
        var ext = portfolioInfo[`file${0}Ext`];
        const filePath = 'data:image/' + ext + ';base64,' + portfolioInfo[`file0`];
        image.src = filePath;
        image.onload = () => {
            elementsToLoad--;
            checkAllLoaded();
        };
        const deleteButton = document.createElement('button');
        deleteButton.textContent = 'Delete Profile Pic';
        deleteButton.addEventListener('click', function() {
            deleteFile(0);
        });
        const mediaSlot = document.getElementById(`file-upload-0`);
        mediaSlot.appendChild(image);
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

    // Set media sources based on file type for each media slot
    for (let i = 1; i <= 5; i++) {
        const fileContent = portfolioInfo[`file${i}`];
        const mediaSlot = document.getElementById(`slot-${i}`);

        if (fileContent == null) {
            //If null then we load upload buttons for media and set genre and desc to display lack media
            const genreParagraph = document.createElement('p');
            genreParagraph.innerHTML = `<strong>Genre: </strong>No file yet`;
            mediaSlot.appendChild(genreParagraph);

            const descParagraph = document.createElement('p');
            descParagraph.innerHTML = `<strong>Desc: </strong>No file yet`;
            mediaSlot.appendChild(descParagraph);
            
            const uploadButton = document.createElement('button');
            uploadButton.textContent = 'Upload Media';
            uploadButton.addEventListener('click', function() {
                triggerFileInput(i);
            });

            //Gotta make input boxes for Genre and Desc input as well
            const genreInput = document.createElement('input');
            genreInput.type = 'text';
            genreInput.id = `genre-input-${i}`;
            genreInput.placeholder = 'Enter Genre';
            mediaSlot.appendChild(genreInput);

            const descInput = document.createElement('input');
            descInput.type = 'text';
            descInput.id = `desc-input-${i}`;
            descInput.placeholder = 'Enter Description';
            mediaSlot.appendChild(descInput);
            mediaSlot.appendChild(uploadButton);
        } else {
            //Create in Genre and Desc paragraphs when not null
            const genre = portfolioInfo[`file${i}Genre`] || 'N/A';
            const desc = portfolioInfo[`file${i}Desc`] || 'N/A';
        
            const genreParagraph = document.createElement('p');
            genreParagraph.innerHTML = `<strong>Genre: </strong>${genre}`;
            mediaSlot.appendChild(genreParagraph);
        
            const descParagraph = document.createElement('p');
            descParagraph.innerHTML = `<strong>Desc: </strong>${desc}`;
            mediaSlot.appendChild(descParagraph);
        
            //Get extension from backend data to create correct media type for base64 strings
            const extension = portfolioInfo[`file${i}Ext`];
            console.log('Extension:', extension);
            const supportedAudioFormats = ['.mp3', '.wav'];
            const supportedVideoFormats = ['.mp4'];
        
            //Dependedt on the extension type it will construct media object to play
            if (supportedAudioFormats.includes(extension) || supportedVideoFormats.includes(extension)) {
                elementsToLoad++;
                const media = extension === '.mp3' ? document.createElement('audio') : document.createElement('video');
                media.src = 'data:audio/' + extension.substring(1) + ';base64,' + fileContent;
                media.controls = true;
                media.style.display = 'block';
                media.onloadeddata = () => {
                    elementsToLoad--;
                    checkAllLoaded();
                };

                //Delete button if user desires to remove this media
                mediaSlot.appendChild(media);
                const deleteButton = document.createElement('button');
                deleteButton.textContent = 'Delete File';
                deleteButton.addEventListener('click', function() {
                    deleteFile(i);
                });
                mediaSlot.appendChild(deleteButton);
        
                // Add play button if it is audio
                if (extension === '.mp3') {
                    const playButton = document.createElement('button');
                    playButton.textContent = 'Play';
                    playButton.addEventListener('click', () => {
                        playMedia(i);
                    });
                    mediaSlot.appendChild(playButton);
                }
            }
        }     
    }

    const checkAllLoaded = () => {
        if (elementsToLoad === 0) {
            // Once all elements are loaded, display the profile view
            document.querySelector('.main').style.display = 'none'; // Hide main contents
            document.getElementById('tempoToolView').style.display = 'none'; // Show tempo tool content
            document.getElementById('artistPortfolioView').style.display = 'block'; // Display artist portfolio
        }
    };
}

function deleteFile(slot){
    var activeUsername = sessionStorage.getItem('username');
    // Send POST request to upload API endpoint
    deleteForm = new FormData();
    deleteForm.append('Username', activeUsername);
    deleteForm.append('SlotNumber', slot);
    fetch('http://localhost:8080/ArtistPortfolio/api/deleteApi', {
        method: 'POST',
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
        document.getElementById('enter-artistPortfolio').click();
    })
    .catch(error => {
        console.error('Error:', error); // Log error message
    });
}

// Modify triggerFileInput to accept slot parameter and pass it with the event
function triggerFileInput(slot) {
    const input = document.getElementById(`media-slot-${slot}-content`);
    input.addEventListener('change', function() {
        const file = input.files[0];

        // Create FormData object to send file and other data
        const formData = new FormData();
        formData.append('username', sessionStorage.getItem('username'));
        formData.append('slot', slot);
        formData.append('file', file);

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
        fetch('http://localhost:8080/ArtistPortfolio/api/uploadApi', {
            method: 'POST',
            body: formData // Remove 'Content-Type' header and send FormData directly
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
            document.getElementById('enter-artistPortfolio').click();
        })
        .catch(error => {
            console.error('Error:', error); // Log error message
        });
    });
    input.click();
}

function resetArtistPortfolioView() {
    // Reset the contents of the artist portfolio view to its default state
    document.getElementById('bio-content').textContent = '';
    document.getElementById('occupation-content').textContent = '';
    document.getElementById('location-content').textContent = '';

    // Remove all media slots
    for (let i = 1; i <= 5; i++) {
        const mediaSlot = document.getElementById(`slot-${i}`);
        mediaSlot.innerHTML = `<p><strong>Media Slot ${i}:</strong></p>
                               <input type="file" id="media-slot-${i}-content" style="display: none;">`;
    }

    // Remove profile pic
    const profilePicSlot = document.getElementById('file-upload-0');
    profilePicSlot.innerHTML = '';
}