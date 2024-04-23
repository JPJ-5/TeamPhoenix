
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('Artist Portfolio View').addEventListener('click', function () {
        var activeUsername = sessionStorage.getItem('username');
        // Reset page to default to remove media when Upload/Deletes occur
        resetArtistPortfolioView();
        loadProfileData(activeUsername);
    });
});


//Calls LoadApi from Controller to load all ArtistProfile data
function loadProfileData(username) {
    var feedbackBox = document.getElementById('portfolio-feedback');
    
    fetch('http://localhost:8080/ArtistPortfolio/api/loadApi', {
        method: 'POST',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(username)
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

    //Creating the Username Display
    const infoParagraph = document.createElement('p');
    infoParagraph.innerHTML = `<strong>${activeUsername}`;
    infoSlot.appendChild(infoParagraph);

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

    // Track the number of elements that need to be loaded
    let elementsToLoad = 0;
    
    //Creating display for the profile picture
    if (portfolioInfo[`file0`]) {
        const mediaSlot = document.getElementById(`file-upload-0`);
        elementsToLoad++;

        //creating image using extension from backend to create right type
        const image = document.createElement('img');
        var ext = portfolioInfo[`file${0}Ext`];
        const filePath = 'data:image/' + ext + ';base64,' + portfolioInfo[`file0`];
        image.src = filePath;
        image.onload = () => {
            elementsToLoad--;
            checkAllLoaded();
        };
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
        const mediaSlot = document.getElementById(`slot-${i}`);

        if (fileContent == null) {
            //If null then we load upload button and input for info and media and set genre and desc to display lack of media
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

            //input boxes for Genre and Desc input if null
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
            //Create display for Genre and Desc paragraphs when not null
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
        
            //check for correct extension and for which exact type to contruct media
            if (supportedAudioFormats.includes(extension) || supportedVideoFormats.includes(extension)) {
                elementsToLoad++;
                const media = extension === '.mp3' ? document.createElement('audio') : document.createElement('video');
                media.src = 'data:audio/' + extension.substring(1) + ';base64,' + fileContent;
                media.controls = true;
                media.style.display = 'block';
                //calling checkAllLoaded to make sure everything is ready before showing
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
        
                // Add play button if it is audio only
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

    //check no more elements loading before displaying
    const checkAllLoaded = () => {
        if (elementsToLoad === 0) {
            // Once all elements are loaded, display the profile view
            document.querySelector('.main').style.display = 'none'; // Hide main contents
            document.getElementById('tempoToolView').style.display = 'none'; // Show tempo tool content
            document.getElementById('ScaleDisplayView').style.display = 'none'; // Show scale display content
            document.getElementById('artistPortfolioView').style.display = 'block'; // Display artist portfolio
        }
    };
}

//function to call api to delete file along with genre and description
function deleteFile(slot){
    var activeUsername = sessionStorage.getItem('username');
    // Send POST request to delete API endpoint
    deleteForm = new FormData();
    deleteForm.append('Username', activeUsername);
    deleteForm.append('SlotNumber', slot);
    fetch('http://localhost:8080/ArtistPortfolio/api/deleteApi', {
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
        document.getElementById('enter-artistPortfolio').click(); //reload page once data is changed in ayway
    })
    .catch(error => {
        console.error('Error:', error); // Log error message
    });
}

//function to call api to delete section in profile info
function deleteInfo(section) {
    var activeUsername = sessionStorage.getItem('username');
    const deleteSection = [activeUsername, `${section}`];

    // Send POST request to delete info API endpoint
    fetch('http://localhost:8080/ArtistPortfolio/api/delInfoApi', {
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
        document.getElementById('enter-artistPortfolio').click(); // reloads page when info is change
    })
    .catch(error => {
        console.error('Error:', error); // Log error message
    });
}

//function to trigger file upload from user including genre and description boxes if used
function triggerFileInput(slot) {
    //checking for inputted media in the input box created for it
    const input = document.getElementById(`media-slot-${slot}-content`);
    input.addEventListener('change', function() {
        const file = input.files[0];

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
        fetch('http://localhost:8080/ArtistPortfolio/api/uploadApi', {
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
            document.getElementById('enter-artistPortfolio').click(); //reloads page when info is changed
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

    // Send POST request to upload API endpoint
    fetch('http://localhost:8080/ArtistPortfolio/api/updateInfoApi', {
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
        document.getElementById('enter-artistPortfolio').click(); //creloading page when data is changed
    })
    .catch(error => {
        console.error('Error:', error); // Log error message
    });
}

function resetArtistPortfolioView() {
    // Remove all media slot objects for reload
    for (let i = 1; i <= 5; i++) {
        const mediaSlot = document.getElementById(`slot-${i}`);
        mediaSlot.innerHTML = `<p><strong>Media Slot ${i}:</strong></p>
                               <input type="file" id="media-slot-${i}-content" style="display: none;">`;
    }

    // Remove profile pic for reload
    const profilePicSlot = document.getElementById('file-upload-0');
    profilePicSlot.innerHTML = '';

    const info = document.getElementById('info-upload');
    info.innerHTML = '';
}

//to be implemented later for users searcing you
//copy of normal display with all input and delete objects taken away
function displayOtherArtiste(portfolioInfo) {
    const infoSlot = document.getElementById(`info-upload`);

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
        const picParagraph = document.createElement('p');
        picParagraph.innerHTML = `<strong> No picture yet`;
        const mediaSlot = document.getElementById(`file-upload-0`);
        mediaSlot.appendChild(picParagraph);
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
            document.getElementById('ScaleDisplayView').style.display = 'none'; // Show scale display content
            document.getElementById('artistPortfolioView').style.display = 'block'; // Display artist portfolio
        }
    };
}