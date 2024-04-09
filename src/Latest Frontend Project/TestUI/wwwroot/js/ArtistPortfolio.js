// Define the playMedia function globally
function playMedia(slot) {
    const mediaSlot = document.getElementById(`slot-${slot}`);
    const media = mediaSlot.querySelector('audio') || mediaSlot.querySelector('video');
    if (media) {
        media.play();
    }
}

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('enter-artistPortfolio').addEventListener('click', function () {
        var activeUsername = sessionStorage.getItem('username');
        var profileDiv = document.getElementById('artistPortfolioView');
        var feedbackBox = document.getElementById('portfolio-feedback');
        // Check if the profile is already displayed

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
    });
});

function displayArtistProfile(portfolioInfo) {
    console.log('Profile info:', portfolioInfo);

    // Update profile info...

    // Set media sources based on file type for each media slot
    for (let i = 0; i <= 5; i++) {
        const fileContent = portfolioInfo[`file${i}`];
        const mediaSlot = document.getElementById(`slot-${i}`);
        const mediaSlotContent = mediaSlot ? mediaSlot.querySelector('span') : null;
        console.log(i + ": " + fileContent);

        if (fileContent == null) {
            if (mediaSlotContent) {
                mediaSlotContent.textContent = 'No file yet';
            }
        } else {
            // Hardcode the file extension for filepath1
            const extension = i === 1 ? '.mp3' : getFileExtension(fileContent);
            console.log('File extension:', extension);

            // Supported file formats
            const supportedAudioFormats = ['.mp3', '.wav'];
            const supportedVideoFormats = ['.mp4'];

            if (supportedAudioFormats.includes(extension)) {
                // Create audio player
                const audio = document.createElement('audio');
                audio.src = 'data:audio/' + extension.substring(1) + ';base64,' + fileContent;
                audio.controls = true;
                audio.style.maxWidth = '100%';
                audio.style.height = 'auto';
                audio.style.display = 'block';
                mediaSlot.appendChild(audio);

                // Add play button for audio files
                const playButton = document.createElement('button');
                playButton.textContent = 'Play';
                playButton.onclick = () => playMedia(i); // Use the globally defined playMedia function
                mediaSlot.appendChild(playButton);
            } else if (supportedVideoFormats.includes(extension)) {
                // Create video player
                const video = document.createElement('video');
                video.src = 'data:video/' + extension.substring(1) + ';base64,' + fileContent;
                video.controls = true;
                video.style.maxWidth = '100%';
                video.style.height = 'auto';
                video.style.display = 'block';
                mediaSlot.appendChild(video);
            }
        }
    }

    document.querySelector('.main').style.display = 'none'; // Hide main contents
    document.getElementById('tempoToolView').style.display = 'none'; // Show tempo tool content
    document.getElementById('artistPortfolioView').style.display = 'block';
}

// Function to extract file extension from file content
function getFileExtension(fileContent) {
    const index = fileContent.indexOf(';base64,');
    if (index !== -1) {
        const mimeType = fileContent.substring(0, index).split(':')[1];
        return '.' + mimeType.split('/')[1];
    }
    return '';
}
