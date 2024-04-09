document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('enter-artistPortfolio').addEventListener('click', function () {
        var activeUsername = sessionStorage.getItem('username');
        var profileDiv = document.getElementById('artistPortfolioView');
        var feedbackBox = document.getElementById('portfolio-feedback');
        // Check if the profile is already displayed

        const query = new URLSearchParams({
            username: activeUsername
        }).toString();
        const url = 'http://localhost:8080/ArtistPortfolio/api/loadApi?' + query.toString();
        fetch(url, {
            method: 'GET',
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

    // Update profile info
    const bioContent = document.getElementById('bio-content');
    const occupationContent = document.getElementById('occupation-content');

    if (bioContent && occupationContent) {
        bioContent.textContent = portfolioInfo.bio || 'No bio yet';
        occupationContent.textContent = portfolioInfo.occupation || 'Unknown';
    } else {
        console.error('Bio content or occupation content element is null.');
        return;
    }

    // Set media sources based on file type for each media slot
    for (let i = 0; i <= 5; i++) {
        const fileContent = portfolioInfo[`file${i}Path`];
        const fileDesc = portfolioInfo[`file${i}Desc`] || 'No file selected';
        const fileGenre = portfolioInfo[`file${i}Genre`] || 'N/A';

        const mediaSlot = document.getElementById(`slot-${i}`);
        const mediaSlotContent = mediaSlot ? mediaSlot.querySelector('span') : null;
        const mediaSlotDesc = mediaSlot ? mediaSlot.querySelector(`#file-desc-${i}`) : null;
        const mediaSlotGenre = mediaSlot ? mediaSlot.querySelector(`#file-genre-${i}`) : null;
        console.log(i + ": " + fileContent)

        if (fileContent == null) {
            if (mediaSlotContent) {
                mediaSlotContent.textContent = 'No file yet';
            }
        } else if (i === 0 && fileContent && (fileContent.endsWith('.png') || fileContent.endsWith('.jpg') || fileContent.endsWith('.jpeg'))) {
            // Image file for slot 0
            const image = document.createElement('img');
            image.src = fileContent;
            image.style.maxWidth = '100%';
            image.style.height = 'auto';
            image.style.display = 'block';
            if (mediaSlot) {
                mediaSlot.insertBefore(image, mediaSlotContent);
            } else {
                console.error(`Media slot ${i} is null.`);
            }
        } else if (i !== 0 && fileContent) {
            // Audio or video file for slots 1 to 5
            if (fileContent.endsWith('.mp3') || fileContent.endsWith('.wav') || fileContent.endsWith('.mp')) {
                // Audio file
                const audio = document.createElement('audio');
                audio.src = fileContent;
                audio.controls = true;
                audio.style.maxWidth = '100%';
                audio.style.height = 'auto';
                audio.style.display = 'block';
                if (mediaSlot) {
                    mediaSlot.insertBefore(audio, mediaSlotContent);
                } else {
                    console.error(`Media slot ${i} is null.`);
                }
            } else if (fileContent.endsWith('.mp4')) {
                // Video file
                const video = document.createElement('video');
                video.src = fileContent;
                video.controls = true;
                video.style.maxWidth = '100%';
                video.style.height = 'auto';
                video.style.display = 'block';
                if (mediaSlot) {
                    mediaSlot.insertBefore(video, mediaSlotContent);
                } else {
                    console.error(`Media slot ${i} is null.`);
                }
            }
        }

        if (i !== 0) {
            if (mediaSlotDesc) {
                mediaSlotDesc.placeholder = 'File Description';
                mediaSlotDesc.value = fileDesc;
            }
            if (mediaSlotGenre) {
                mediaSlotGenre.placeholder = 'File Genre';
                mediaSlotGenre.value = fileGenre;
            }
        }
    }

    document.querySelector('.main').style.display = 'none'; // Hide main contents
    document.getElementById('tempoToolView').style.display = 'none'; // Show tempo tool content
    document.getElementById('artistPortfolioView').style.display = 'block';
}

function uploadFile(slotNumber) {
    // Implement file upload logic
}

function deleteFile(slotNumber) {
    // Implement file deletion logic
}