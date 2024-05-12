function setupArtistProfileCalendar() {
    //var baseUrl = 'http://localhost:8080';
    var baseUrl = 'https://themusicali.com:5000';
    document.getElementById('enter-calendar').addEventListener('click', function () {
        // display the artist calendar section when the button is clicked
        document.getElementById('artist-calendar-section').style.display = 'block';
    });

    document.getElementById('artist-calendar-visibility').addEventListener('click', function () {
        closeAllForms()
        var form = document.getElementById('artist-calendar-visibility-form')
        form.style.display = 'block';
    });

    document.getElementById('artist-calendar-visibility-save-database').addEventListener('click', function () {
        // get the username which is needed for gig updating.
        var username = sessionStorage.getItem('username'); // Make sure this is correctly retrieving the username
        var form = document.getElementById('artist-calendar-visibility-form');
        var feedbackBox = document.getElementById('artist-calendar-visibility-feedback');

        // Check if the form is already displayed
        if (form.style.display === 'block') {
            // Form is displayed, try to submit form data
            const visibility = form.querySelector('#calendarVisibilityUpdate').checked;

            var payload = {
                username: username,
                gigVisibility: visibility
            }

            fetch(`${baseUrl}/ArtistCalendar/api/ArtistCalendarGigVisibilityAPI`, {
                method: 'POST',
                headers: {
                    'Authentication': idToken,
                    'Authorization': accessToken,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(payload),
            })
                .then(response => response.json())
                .then(result => {
                    feedbackBox.style.display = 'block';
                    if (result) {
                        feedbackBox.textContent = 'Calendar Visibilty updated successfully';
                        feedbackBox.style.color = 'green';
                        form.reset();
                    } else {
                        feedbackBox.textContent = 'Failed to update Calendar Visibilty';
                        feedbackBox.style.color = 'red';
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    feedbackBox.textContent = 'Error updating Calendar Visibilty. Please try again.';
                    feedbackBox.style.color = 'red';
                });
        } else {
            // Form is currently not displayed, so display it
            form.style.display = 'block';
        }
    });

    document.getElementById('artist-calendar-view-gig').addEventListener('click', function () {
        closeAllForms()
        var form = document.getElementById('view-gig-form')
        form.style.display = 'block';
    });

    document.getElementById('view-gig-from-database').addEventListener('click', function () {
        var activeUsername = sessionStorage.getItem('username');
        var form = document.getElementById('view-gig-form');
        var feedbackBox = document.getElementById('view-gig-feedback');
        // Check if the form is already displayed
        if (form.style.display === 'block') {
            const usernameOwner = form.querySelector('#gigUsernameOwnerView').value;
            const gigDate = form.querySelector('#gigDateView').value;
            const queryView = new URLSearchParams({
                username: activeUsername,
                usernameOwner: usernameOwner,
                dateOfGig: gigDate
            }).toString();
            url = `${baseUrl}/ArtistCalendar/api/ArtistCalendarGigViewAPI?` + queryView.toString();
            fetch(url, {
                method: 'GET',
                headers: {
                    'Authentication': idToken,
                    'Authorization': accessToken
                }
            })
                .then(response => {
                    if (response.ok) {
                        return response.json();
                    } else {
                        throw new Error('Failed to view gig');
                    }
                })
                .then(gigView => {
                    displayGigView(gigView);
                })
                .catch(error => {
                    console.error('Error:', error);
                    feedbackBox.textContent = 'Error viewing gig. Please try again.';
                    feedbackBox.style.color = 'red';
                });
        } else {
            form.style.display = 'block';
        }
    });
    function displayGigView(gigView) {
        // Dummy data for gigView, replace with real data as needed
        document.getElementById('artist-calendar-view-username').textContent = gigView.username || 'N/A';
        document.getElementById('artist-calendar-view-gig-name').textContent = gigView.gigName || 'N/A';
        document.getElementById('artist-calendar-view-date').textContent = gigView.dateOfGig || 'N/A';
        document.getElementById('artist-calendar-view-description').textContent = gigView.description || 'N/A';
        document.getElementById('artist-calendar-view-location').textContent = gigView.location || 'N/A';
        document.getElementById('artist-calendar-view-pay').textContent = gigView.pay || 'N/A';

        document.getElementById('view-gig-listing').style.display = 'block';
    }

    document.getElementById('artist-calendar-delete-gig').addEventListener('click', function () {
        closeAllForms()
        var form = document.getElementById('delete-gig-form');
        form.style.display = 'block';
    });

    document.getElementById('delete-gig-from-database').addEventListener('click', function () {
        // get the username which is needed for gig deletion.
        var username = sessionStorage.getItem('username'); // Make sure this is correctly retrieving the username
        var form = document.getElementById('delete-gig-form');
        var feedbackBox = document.getElementById('delete-gig-feedback');
        // Check if the form is already displayed
        if (form.style.display === 'block') {
            const gigDate = form.querySelector('#gigDateDelete').value;
            var payload = {
                username: username,
                dateOfGig: gigDate
            }
            fetch(`${baseUrl}/ArtistCalendar/api/ArtistCalendarGigDeletionAPI`, {
                method: 'DELETE',
                headers: {
                    'Authentication': idToken,
                    'Authorization': accessToken,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(payload),
            })
                .then(response => {
                    if (response.ok) {
                        return response.json();
                    } else {
                        throw new Error('Failed to delete gig');
                    }
                })
                .then(data => {
                    feedbackBox.style.display = 'block';
                    feedbackBox.textContent = 'Gig deleted successfully';
                    feedbackBox.style.color = 'green';
                    form.reset();
                })
                .catch(error => {
                    console.error('Error:', error);
                    feedbackBox.textContent = 'Error deleting gig. Please try again.';
                    feedbackBox.style.color = 'red';
                });
        } else {
            form.style.display = 'block';
        }

    });

    document.getElementById('artist-calendar-update-gig').addEventListener('click', function () {
        closeAllForms()
        var form = document.getElementById('update-gig-form');
        form.style.display = 'block';
    });

    document.getElementById('update-gig-save-database').addEventListener('click', function () {
        // get the username which is needed for gig updating.
        var username = sessionStorage.getItem('username'); // Make sure this is correctly retrieving the username
        var form = document.getElementById('update-gig-form');
        var feedbackBox = document.getElementById('update-gig-feedback');

        // Check if the form is already displayed
        if (form.style.display === 'block') {
            // Form is displayed, try to submit form data
            const gigDateOld = form.querySelector('#gigDateOriginal').value;
            const gigName = form.querySelector('#gigNameUpdate').value;
            const gigDate = form.querySelector('#gigDateUpdate').value;
            const gigVisibility = form.querySelector('#gigVisibilityUpdate').checked;
            const gigDescription = form.querySelector('#gigDescriptionUpdate').value;
            const gigLocation = form.querySelector('#gigLocationUpdate').value;
            const gigPay = form.querySelector('#gigPayUpdate').value;

            var payload = {
                dateOfGigOriginal: gigDateOld,
                username: username,
                gigName: gigName,
                dateOfGig: gigDate,
                visibility: gigVisibility,
                description: gigDescription,
                location: gigLocation,
                pay: gigPay
            }

            fetch(`${baseUrl}/ArtistCalendar/api/ArtistCalendarGigUpdateAPI`, {
                method: 'POST',
                headers: {
                    'Authentication': idToken,
                    'Authorization': accessToken,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(payload),
            })
                .then(response => response.json())
                .then(result => {
                    feedbackBox.style.display = 'block';
                    if (result) {
                        feedbackBox.textContent = 'Gig updated successfully';
                        feedbackBox.style.color = 'green';
                        form.reset();
                    } else {
                        feedbackBox.textContent = 'Failed to update gig';
                        feedbackBox.style.color = 'red';
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    feedbackBox.textContent = 'Error updating gig. Please try again.';
                    feedbackBox.style.color = 'red';
                });
        } else {
            // Form is currently not displayed, so display it
            form.style.display = 'block';
        }
    });

    document.getElementById('artist-calendar-create-gig').addEventListener('click', function () {
        // get the username which is needed for gig creation.
        var username = sessionStorage.getItem('username'); // Make sure this is correctly retrieving the username
        closeAllForms()
        var form = document.getElementById('create-gig-form');
        form.style.display = 'block';
    });

    document.getElementById('create-gig-save-database').addEventListener('click', function () {
        // get the username which is needed for gig creation.
        var username = sessionStorage.getItem('username'); // Make sure this is correctly retrieving the username
        var form = document.getElementById('create-gig-form');
        var feedbackBox = document.getElementById('create-gig-feedback');

        // Check if the form is already displayed
        if (form.style.display === 'block') {
            // Form is displayed, try to submit form data
            const gigName = form.querySelector('#gigName').value;
            const gigDate = form.querySelector('#gigDate').value;
            const gigVisibility = form.querySelector('#gigVisibility').checked;
            const gigDescription = form.querySelector('#gigDescription').value;
            const gigLocation = form.querySelector('#gigLocation').value;
            const gigPay = form.querySelector('#gigPay').value;

            var payload = {
                username: username,
                gigName: gigName,
                dateOfGig: gigDate,
                visibility: gigVisibility,
                description: gigDescription,
                location: gigLocation,
                pay: gigPay
            }

            fetch(`${baseUrl}/ArtistCalendar/api/ArtistCalendarGigCreationAPI`, {
                method: 'POST',
                headers: {
                    'Authentication': idToken,
                    'Authorization': accessToken,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(payload),
            })
                .then(response => response.json())
                .then(result => {
                    feedbackBox.style.display = 'block';
                    if (result) {
                        feedbackBox.textContent = 'Gig created successfully';
                        feedbackBox.style.color = 'green';
                        form.reset();
                    } else {
                        feedbackBox.textContent = 'Failed to create gig';
                        feedbackBox.style.color = 'red';
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    feedbackBox.textContent = 'Error creating gig. Please try again.';
                    feedbackBox.style.color = 'red';
                });
        } else {
            // Form is currently not displayed, so display it
            form.style.display = 'block';
        }
    });

    function closeAllForms() {
        var form = document.getElementById('artist-calendar-visibility-form')
        form.style.display = 'none';
        var form = document.getElementById('delete-gig-form');
        form.style.display = 'none';
        var form = document.getElementById('artist-calendar-visibility-form');
        form.style.display = 'none';
        var form = document.getElementById('view-gig-form')
        form.style.display = 'none';
        document.getElementById('view-gig-listing').style.display = 'none';
        var form = document.getElementById('update-gig-form');
        form.style.display = 'none';
        var form = document.getElementById('create-gig-form');
        form.style.display = 'none';
    }
}