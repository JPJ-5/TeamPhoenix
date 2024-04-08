document.getElementById('enter-artistPortfolio').addEventListener('click', function () {
    var activeUsername = sessionStorage.getItem('username');
    var profileDiv = document.getElementById('artistPortfolioView');
    var feedbackBox = document.getElementById('portfolio-feedback');
    // Check if the profile is already displayed
    if (profileDiv.style.display === 'block') {
        const query = new URLSearchParams({
            username: activeUsername
        }).toString();
        const url = 'http://localhost:8080/ArtistPortfolio/api/loadApi?' + query;
        fetch(url, {
            method: 'GET',
        })
            .then(response => {
                if (response.ok) {
                    return response.json();
                } else {
                    throw new Error('Failed to load artist profile');
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
    } else {
        profileDiv.style.display = 'block';
    }
});

function displayArtistProfile(profileData) {
    const profileInfo = profileData.List1;
    const localFileInfo = profileData.List2;
    console.log('Profile info:', ProfileInfo);
    console.log('Profile occ:', ProfileInfo[0]);

    // Update profile info
    document.getElementById('bio-content').textContent = profileInfo[1] || 'N/A';
    document.getElementById('occupation-content').textContent = profileInfo[0] || 'N/A';

    // Update profile pic
    const profilePicContent = document.getElementById('media-slot-0-content');
    if (localFileInfo && localFileInfo.length > 0 && localFileInfo[0].length > 0) {
        profilePicContent.textContent = 'Profile Pic';
    } else {
        profilePicContent.textContent = 'N/A';
    }

    // Update media slots
    for (let i = 1; i <= 5; i++) {
        const mediaSlotContent = document.getElementById(`media-slot-${i}-content`);
        if (localFileInfo && localFileInfo.length >= 1 && localFileInfo[0].length >= i) {
            mediaSlotContent.textContent = `${localFileInfo[0][i - 1]}` || 'N/A';
        } else {
            mediaSlotContent.textContent = 'N/A';
        }
    }

    // Update genre and description for media slots
    for (let i = 1; i <= 5; i++) {
        const genreContent = document.getElementById(`file-genre-${i}`);
        const descContent = document.getElementById(`file-desc-${i}`);
        if (localFileInfo && localFileInfo.length >= 3 && localFileInfo[1].length >= i && localFileInfo[2].length >= i) {
            genreContent.textContent = `${localFileInfo[1][i - 1]}` || 'N/A';
            descContent.textContent = `${localFileInfo[2][i - 1]}` || 'N/A';
        } else {
            genreContent.textContent = 'N/A';
            descContent.textContent = 'N/A';
        }
    }
}

