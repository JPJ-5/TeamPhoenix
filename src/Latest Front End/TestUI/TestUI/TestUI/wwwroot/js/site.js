document.addEventListener('DOMContentLoaded', function () {
    var menuButton = document.getElementById('menu-btn');
    var dropdown = document.getElementById('myDropdown');
    var showLoginFormButton = document.getElementById('show-login');
    var loginForm = document.getElementById('login-form');
    var showRegisterButton = document.getElementById('show-register');
    var registerEmailForm = document.getElementById('register-email-form');
    var showDetailsFormButton = document.getElementById('show-details-form');
    var registerDetailsForm = document.getElementById('register-details-form');
    var showRecoveryButton = document.getElementById('account-recovery-button');


    menuButton.addEventListener('click', function () {
        dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
        // Reset the visibility of login and register buttons when menu is toggled
        resetButtonVisibility();
    });

    showLoginFormButton.addEventListener('click', function () {
        hideGroup('group2');
        hideGroup('group3');
        loginForm.style.display = 'block';
        showRegisterButton.style.display = 'none'; // Hide the register button
        showRecoveryButton.style.display = 'none';
    });

    //showOtpButton.addEventListener('click', function () {
    //    hideGroup('group2');
    //    hideGroup('group3');
    //    otpForm.style.display = 'block';
    //});

    showRegisterButton.addEventListener('click', function () {
        hideGroup('group1');
        hideGroup('group3');
        registerEmailForm.style.display = 'block';
        showLoginFormButton.style.display = 'none'; // Hide the login button
        showRecoveryButton.style.display = 'none';
    });

    showDetailsFormButton.addEventListener('click', function () {
        hideGroup('group1');
        hideGroup('group3');
        registerDetailsForm.style.display = 'block';
    });

    showRecoveryButton.addEventListener('click', function () {
        hideGroup('group1');
        hideGroup('group2');
        showLoginFormButton.style.display = 'none'; // Hide the login button
        showRegisterButton.style.display = 'none';
    });

    document.getElementById("email-otp").addEventListener("click", function () {
        //var email = document.getElementById("email").value;
        var username = document.getElementById("username").value;
        // AJAX request to backend
        fetch('http://localhost:8080/Login/api/CheckUsernameAPI?', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ username: username, otp: "string" })
        })
            .then(response => response.json())
            .then(exists => {
                if (exists) {
                    // Email exists and OTP sent
                    alert("OTP sent to your email.");
                    // Optionally, show OTP form

                    var otpVal = "0000";
                    //call OTP display controller here
                    /*
                    fetch('http://localhost:8080/Login/api/FetchOTP', {
                        method: '',
                        headers: {
                            'Content-Type': 'application/json',
                        },
                        body: JSON.stringify({ username: username, otp: "string" })
                    })*/
                    
                    document.getElementById("otp-form").style.display = 'block';

                    //displays loaded OTP val
                    var otpDisplay = document.getElementById('otp-display-val');
                    otpDisplay.textContent = otpVal;
                    document.getElementById('otp-display-val').style.display = 'block';

                } else {
                    // Email does not exist
                    alert("Email does not exist.");
                }
            })
            .catch((error) => {
                console.error('Error:', error);
            });
    });


    // Add event listener for OTP form submission

    document.getElementById("submit-otp").addEventListener("click", function (event) {
        event.preventDefault(); // Prevent the default form submission

        var username = document.getElementById("username").value;
        var otp = document.getElementById("enter-otp").value;

        // AJAX request to the backend
        fetch('http://localhost:8080/Login/api/GetJwtAPI', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ username: username, otp: otp })
        })
            .then(response => {
                // Check if the response is JSON
                var contentType = response.headers.get("content-type");
                if (contentType && contentType.indexOf("application/json") !== -1) {
                    return response.json().then(data => {
                        if (data.success && data.token) {
                            // Handle JSON response
                            localStorage.setItem("jwt", data.token);
                            //fetchAndDisplayUserProfile(data.token);
                            displayUserProfile()
                        } else {
                            alert("Invalid OTP or error occurred.");
                        }
                    });
                } else {
                    return response.text().then(token => {
                        // Handle plain text response
                        localStorage.setItem("jwt", token);
                        //fetchAndDisplayUserProfile(token);
                        displayUserProfile()
                    });
                }
            })
            .catch((error) => {
                console.error('Error:', error);
                alert("An error occurred.");
            });
    });


    function fetchAndDisplayUserProfile(token) {
        fetch('http://localhost:8080/AccCreationAPI/api/NormalAccCreationAPI?', {
            headers: {
                'Authorization': 'Bearer ' + token
            }
        })
            .then(response => response.json())
            .then(userProfile => {
                // Display the user profile
                displayUserProfile(userProfile);
            });
    }

    function displayUserProfile(userProfile) {
        // Update the UI with user profile information
        document.getElementById("username").textContent = userProfile.username;
        // ...display other user info
    }

    document.getElementById('otp-form').addEventListener('submit', function (event) {
        event.preventDefault();
        // Hide forms and display user profile
        hideGroup('group1');
        hideGroup('group2');
        hideGroup('group3');
        displayUserProfile(); // Function to populate and show the user profile
    });



    document.getElementById('account-recovery-button').addEventListener('click', function (event) {
        event.preventDefault();
        document.getElementById('account-recovery-section').style.display = 'block';
    });

    // Event listener for submit-recovery-email
    document.getElementById('submit-recovery-username').addEventListener('click', function (event) {
        event.preventDefault();
        var userName = document.getElementById('username').value;

        // Using the fetch API to send the userName in the request headers
        fetch("/api/RecoverUser", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ UserName: userName }),
        })
            .then(response => {
                // Check if the request was successful
                if (response.ok) {
                    return response.json(); // Parse the JSON response
                }
                throw new Error('Network response was not ok.'); // Handle HTTP errors
            })
            .then(data => {
                // Here, data is the JSON object returned by the server
                if (data.hasOwnProperty(true)) {
                    // Logic for a successful recovery initiation
                    console.log(data[true]); // Log the success message
                    document.getElementById('otp-recovery-section').style.display = 'block';
                } else {
                    // Handle failure
                    console.error("Unable to recover user:", data[false]);
                }
            })
            .catch(error => {
                // Handle any errors that occurred during the fetch
                console.error('There has been a problem with your fetch operation:', error);
            });
    });



    // Event listener for submit-recovery-otp
    //document.getElementById('submit-recovery-otp').addEventListener('click', function () {
    //    event.preventDefault();
    //    var otp = document.getElementById('recovery-otp').value;
    //    // Implement logic to verify the OTP
    //    // If OTP is correct, notify the user about reactivation process
    //    alert('OTP verified. Your account will be reviewed for reactivation by an admin.');
    //});

    function displayUserProfile() {
        // Dummy data for user profile, replace with real data as needed
        document.getElementById('user-last-name').textContent = 'An';
        document.getElementById('user-first-name').textContent = 'Khuong';
        document.getElementById('user-email').textContent = 'helloworld@gmail.com';
        document.getElementById('user-dob').textContent = '2024-01-01';
        document.getElementById('user-role').textContent = 'rootAdmin';
        document.getElementById('user-status').textContent = 'Active';
        document.getElementById('user-description').textContent = 'A brief description here.';
        document.getElementById('user-profile-pic').src = 'profilePic.jpg'; // Update the path to the profile picture

        document.getElementById('user-profile').style.display = 'block';
        // Assuming 'editBtn' is your edit button and it's hidden by default,
        // set it to be visible along with the user profile.
       // document.getElementById('editBtn').style.display = 'block';
       // var userRole = document.getElementById('user-role').textContent;
        //var element = document.getElementById("username");
        
        //if (element) {
        //    element.textContent = userProfile.username;
        //} else {
        //    console.error("Element not found: username");
        //}
        //if (userRole === 'rootAdmin') {
        //    document.getElementById('create-admin-button').style.display = 'block';
        //    document.getElementById('modify-account-button').style.display = 'block';
        //}
    }

    document.getElementById('edit-button').addEventListener('click', function () {
        // Show the edit-profile div
        document.getElementById('edit-profile').style.display = 'block';

        // Hide the displayed first and last names
        document.getElementById('user-first-name').style.display = 'none';
        document.getElementById('user-last-name').style.display = 'none';

        // Hide the edit button
        this.style.display = 'none';
    });

    // Save button functionality
    document.getElementById('save-button').addEventListener('click', function () {
        // Update the name display
        document.getElementById('user-first-name').textContent = document.getElementById('edit-first-name').value;
        document.getElementById('user-last-name').textContent = document.getElementById('edit-last-name').value;

        // Hide the edit-profile div
        document.getElementById('edit-profile').style.display = 'none';

        // Show the first and last names and the edit button
        document.getElementById('user-first-name').style.display = 'block';
        document.getElementById('user-last-name').style.display = 'block';
        document.getElementById('edit-button').style.display = 'block';
    });

    // Cancel button functionality
    document.getElementById('cancel-button').addEventListener('click', function () {
        // Hide the edit-profile div
        document.getElementById('edit-profile').style.display = 'none';

        // Show the first and last names and the edit button
        document.getElementById('user-first-name').style.display = 'block';
        document.getElementById('user-last-name').style.display = 'block';
        document.getElementById('edit-button').style.display = 'block';
    });

    document.getElementById('create-admin-button').addEventListener('click', function () {
        document.getElementById('create-admin-section').style.display = 'block';
        this.style.display = 'none'; // Optionally hide the "Create Admin" button
    });
    document.getElementById('submit-admin').addEventListener('click', function () {
        var firstName = document.getElementById('admin-first-name').value;
        var lastName = document.getElementById('admin-last-name').value;
        var email = document.getElementById('admin-email').value;
        var role = document.getElementById('admin-role').value;

        // Implement the logic to create an admin account
        // This could involve sending a request to your server

        // Optionally reset the form and hide the admin creation section
        document.getElementById('create-admin-section').style.display = 'none';
        document.getElementById('create-admin-button').style.display = 'block';
    });

    document.getElementById('modify-account-button').addEventListener('click', function () {
        document.getElementById('account-search-section').style.display = 'block';
    });


    document.getElementById('logoutButton').addEventListener('click', function () {
        const startTime = Date.now();
        localStorage.removeItem('jwt');
        var userName = document.getElementById("username").value;

        fetch('http://localhost:8080/Logout/api/logout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ UserName: userName }),
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Logout operation error');
                }
                return response.json();
            })
            .then(data => {
                const duration = Date.now() - startTime;
                if (duration > 3000) {
                    throw new Error('Logout took longer than 3 seconds.');
                }
                alert(data.message); // Display logout message from server
                // Redirect to home page with default culture settings
                window.location.href = 'index.html'; // Adjust this to your home page URL
            })
            .catch(error => {
                console.error('Error:', error);
                alert(error.message); // Display error message to the user
            });
    });


    document.getElementById('search-account').addEventListener('click', function () {
        var emailToSearch = document.getElementById('search-email').value;
        // Dummy condition to simulate email match. Replace this with your actual search logic.
        if (emailToSearch === "ankhuong@email.com") {
            document.getElementById('edit-account-section').style.display = 'block';
        } else {
            // Optionally, handle the case where no matching email is found
            alert('No account found with that email.');
        }
    });

    // Event listener for save-changes-button
    document.getElementById('save-changes-button').addEventListener('click', function () {
        var updatedFirstName = document.getElementById('edit-user-first-name').value;
        var updatedLastName = document.getElementById('edit-user-last-name').value;
        var updatedRole = document.getElementById('edit-user-role').value;

        // AJAX request to server to save the updated account information
        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/update-account", true); // Replace with your actual server URL
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.onreadystatechange = function () {
            if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
                // Handle successful update
                alert('Account updated successfully.');
                // Optionally reset the UI or perform other actions
            }
        }
        xhr.send(JSON.stringify({
            firstName: updatedFirstName,
            lastName: updatedLastName,
            role: updatedRole
        }));
        document.getElementById('edit-account-section').style.display = 'none'
    });

    // Event listener for delete-account-button
    document.getElementById('delete-account-button').addEventListener('click', function () {
        // Implement logic to delete the account
        var emailToDelete = document.getElementById('search-email').value;

        // Confirm with the user before deletion
        if (confirm('Are you sure you want to delete the account for ' + emailToDelete + '?')) {
            // AJAX request to server for account deletion
            var xhr = new XMLHttpRequest();
            xhr.open("POST", "/delete-account", true); // Replace with your actual server URL
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onreadystatechange = function () {
                if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
                    // Handle successful deletion
                    alert('Account deleted successfully.');
                    // Optionally reset UI or redirect
                }
            }
            xhr.send(JSON.stringify({ email: emailToDelete }));
        }
        document.getElementById('edit-account-section').style.display = 'none'
    });




    window.onclick = function (event) {
        if (!event.target.matches('.hamburger, .bar, .dropdown, .dropdown *')) {
            dropdown.style.display = 'none';
            hideGroup('group1');
            hideGroup('group2');
            hideGroup('group3');
            resetButtonVisibility();
        }
    };

    function hideGroup(groupName) {
        var elements = document.querySelectorAll('.' + groupName);
        elements.forEach(function (element) {
            element.style.display = 'none';
        });
    }

    function resetButtonVisibility() {
        showLoginFormButton.style.display = 'block';
        showRegisterButton.style.display = 'block';
    }


    document.getElementById('finalize-registration').addEventListener('click', function (event) {
        event.preventDefault();

        // Collect data from the form
        var email = document.getElementById('register-email').value;
        var dob = document.getElementById('dob').value;
        var uname = document.getElementById('user-name').value;
        var bmail = document.getElementById('backup-email').value;

        // Create a URLSearchParams object to encode the data
        var params = new URLSearchParams();
        params.append('email', email);
        params.append('dob', dob);
        params.append('uname', uname);
        params.append('bmail', bmail);

        // Construct the URL with the encoded parameters
        var url = 'http://localhost:8080/AccCreationAPI/api/NormalAccCreationAPI?' + params.toString();

        // Log the URL
        console.log('URL:', url);

        // Send data to your API
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
        })
            .then(response => response.json())
            .then(data => {
                console.log('Success:', data);
                // Handle success (e.g., show success message)
            })
            .catch((error) => {
                console.error('Error:', error);
                // Handle error (e.g., show error message)
            });
    });


});