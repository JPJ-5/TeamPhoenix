document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('enter-artistPortfolio').addEventListener('click', function () {
        var activeUsername = sessionStorage.getItem('username');
        var profileDiv = document.getElementById('artistPortfolioView');
        var feedbackBox = document.getElementById('portfolio-feedback');

        const query = new URLSearchParams({
            username: activeUsername
        }).toString();
        const url = `http://localhost:8080/ArtistPortfolio/api/loadApi?${query}`;
        fetch(url)
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
    // Update profile info...

    // Display files
    for (let i = 0; i <= 5; i++) {
        const fileContent = portfolioInfo[`file${i}Path`];
        if (fileContent) {
            const mediaSlot = document.getElementById(`slot-${i}`);
            if (mediaSlot) {
                if (fileContent.endsWith('.mp3') || fileContent.endsWith('.wav') || fileContent.endsWith('.mp')) {
                    // Audio file
                    const audio = document.createElement('audio');
                    audio.src = `data:audio/mpeg;base64,${fileContent}`;
                    audio.controls = true;
                    audio.style.maxWidth = '100%';
                    audio.style.height = 'auto';
                    audio.style.display = 'block';
                    mediaSlot.appendChild(audio);
                } else if (fileContent.endsWith('.mp4')) {
                    // Video file
                    const video = document.createElement('video');
                    video.src = `data:video/mp4;base64,${fileContent}`;
                    video.controls = true;
                    video.style.maxWidth = '100%';
                    video.style.height = 'auto';
                    video.style.display = 'block';
                    mediaSlot.appendChild(video);
                } else if (fileContent.endsWith('.png') || fileContent.endsWith('.jpg') || fileContent.endsWith('.jpeg')) {
                    // Image file
                    const image = document.createElement('img');
                    image.src = `data:image/jpeg;base64,${fileContent}`;
                    image.style.maxWidth = '100%';
                    image.style.height = 'auto';
                    image.style.display = 'block';
                    mediaSlot.appendChild(image);
                }
            }
        }
    }

    // Hide main content and show artist portfolio view
    document.querySelector('.main').style.display = 'none';
    document.getElementById('tempoToolView').style.display = 'none';
    document.getElementById('artistPortfolioView').style.display = 'block';
}


function uploadFile(slotNumber) {
    // Implement file upload logic
}

function deleteFile(slotNumber) {
    // Implement file deletion logic
}
