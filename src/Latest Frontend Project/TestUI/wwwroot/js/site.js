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
    var baseUrl = 'http://localhost:8080';
    var idToken;
    var accessToken;



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
        localStorage.clear()
        sessionStorage.clear()
        //var email = document.getElementById("email").value;
        var username = document.getElementById("username").value;

        var checkUsernameApiUrl = baseUrl + '/Login/api/CheckUsernameAPI?';

        // AJAX request to backend
        fetch(checkUsernameApiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ username: username, otp: "string" })
        })
            .then(response => response.json())
            .then(exists => {
                console.log(exists); // Log the parsed JSON data
                if (exists) {
                    // Email exists and OTP sent
                    alert("OTP sent to your email.");
                    // Optionally, show OTP form
                    document.getElementById("otp-form").style.display = 'block';
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
        sessionStorage.setItem('username', username);
        var otp = document.getElementById("enter-otp").value;

        var getJwtApiUrl = baseUrl + '/Login/api/GetJwtAPI';

        // AJAX request to the backend
        fetch(getJwtApiUrl, {
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
                        //if (data.success && data.token) {
                        if (data.IdToken && data.AccessToken) {
                            // Handle JSON response
                            //sessionStorage.setItem("jwt", data.token);
                            sessionStorage.setItem("idToken", data.IdToken);
                            sessionStorage.setItem("accessToken", data.AccessToken);
                            idToken = data.idToken;
                            accessToken = data.accessToken;
                            fetchUserProfile(username)
                        } else {
                            alert("Invalid OTP or error occurred.");
                        }
                    });
                } else {
                    return response.text().then(token => {
                        // Handle plain text response
                        sessionStorage.setItem("jwt", token);
                        if (sessionStorage.getItem('idToken', token) != "Login Failed") {
                            
                            fetchUserProfile(username)
                        }
                        else {
                            alert("Invalid OTP")
                        }
                    });
                }
            })
            .catch((error) => {
                console.error('Error:', error);
                alert("An error occurred.");
            });
    });

    function fetchUserProfile(username) {
        var username = document.getElementById('username').value;
        var userProfileUrl = `${baseUrl}/ModifyUserProfile/GetUserInformation`;
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");
        //console.log(idToken);
        //console.log(accessToken);
        
        fetch(userProfileUrl, {
            method: 'GET',
            headers: {
                'userName': username,
                'Authentication': idToken,
                'Authorization': accessToken
            }
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(userProfile => {
            displayUserProfile(userProfile);
        })
        .catch(error => {
            console.error('Failed to fetch user profile:', error);
        });
    }

    // After successful login or when displaying the user profile, call this function to fetch and set the user role
    function fetchAndSetUserRole(username) {
        var username = document.getElementById('username').value;
        // Assuming the base URL and the necessary route to your controller
        var getUserRoleUrl = `${baseUrl}/ModifyUserProfile/GetUserInformation`;
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");
        //console.log(idToken);
        //console.log(accessToken);
        fetch(getUserRoleUrl, {
            method: 'GET',
            headers: {
                'userName': username,
                'Authentication': idToken,
                'Authorization': accessToken
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Failed to fetch user role');
                }
                return response.json();
            })
            .then(data => {
                // Assuming data contains { username: "username", role: "userRole" }
                // Now you can set the role in the UI and adjust visibility
                document.getElementById('user-role').textContent = data.userRole;
                adjustUIBasedOnRole(data.userRole);
            })
            .catch(error => {
                console.error('Error fetching user role:', error);
            });
    }

    // Function to adjust UI based on the role
    function adjustUIBasedOnRole(userRole) {
        if (userRole === 'NormalUser') {
            prepareNormalUserUI()
        } else if (userRole === 'AdminUser') {
            prepareAdminUI()
            
        }
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
        var userName = document.getElementById('User Name').value;
        var recoverUserUrl = `${baseUrl}/api/RecoverUser`;
        console.log(userName);
        // Prepare the headers
        var headers = new Headers();
        headers.append("userName", userName); // Assuming the ID of the input is 'User Name', which is unusual due to the space. Consider changing this ID to 'user-name' or similar for better consistency.
        //headers.append("Authentication", idToken);
        //headers.append("Authorization", accessToken);
        //Prepare the request options
        var requestOptions = {
            method: 'POST',
            headers: headers
        };

        // Make the fetch call to the API
        fetch(recoverUserUrl, requestOptions)
            .then(response => response.json()) // Parse JSON response
            .then(result => {
                console.log(result);
                if (result) {
                    // Success logic here
                    alert("Success Recover")
                    document.getElementById('otp-recovery-section').style.display = 'block';
                } else {
                    alert("Invalid Username")
                }
            })
            .catch(error => console.log('error', error)); // Handle any errors that occur during the fetch.
    });

    document.getElementById('otp-recovery-section').addEventListener('submit', function (event) {
        event.preventDefault(); // Prevent the form from submitting

        const otpInput = document.getElementById('re-enter-otp').value.trim();
        if (otpInput === '') {
            alert("Please enter the OTP.");
            return false;
        } else {
            console.log('OTP entered:', otpInput);
            alert('OTP Submitted. Now you can login in.');
            // Here you would typically handle the OTP, such as sending it to the server for verification
            logoutUser()
        }
    });

    // Prepare UI for a normal user
    function prepareNormalUserUI() {
        // Show modify profile options for normal user
        document.getElementById('normal-user-modify-section').style.display = 'block';
        // Hide admin management section just in case it was previously shown
        document.getElementById('admin-management-section').style.display = 'none';
    }

    // Inside prepareNormalUserUI
    document.getElementById('normal-user-delete').addEventListener('click', function () {
        var username = sessionStorage.getItem('username');
        //var idToken = sessionStorage.getItem("idToken");
        //var accessToken = sessionStorage.getItem("accessToken");
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");


        if (username && idToken && accessToken) {
            var deleteUserUrl = `${baseUrl}/ModifyUserProfile/DeleteProfile`;
            fetch(deleteUserUrl, {
                method: 'DELETE',
                headers: {
                    'Authentication': idToken,
                    'Authorization': accessToken,
                    'userName': username                    
                }
            })
                .then(response => {
                    if (response.ok) {
                        return response.text();
                    } else {
                        throw new Error('Failed to delete user.');
                    }
                })
                .then(data => {
                    alert('User deleted successfully.');
                    logoutUser(); // Call a function to handle the logout process
                })
                .catch(error => {
                    console.error('Error deleting user:', error);
                    alert(error.message);
                });
        } else {
            alert('Error: User information not found.');
        }
    });

    document.getElementById('enter-priceRangeSorting').addEventListener('click', function () {
        window.location.href = 'PriceRangeSorting.html'; // Redirects the user to PriceRangeSorting.html
    });

    function logoutUser() {
        localStorage.clear()
        sessionStorage.clear()
        // Optionally, you can also invalidate the token on the server side here

        // Redirect to the homepage
        window.location.href = 'index.html';
    }

    document.getElementById('normal-user-save-changes').addEventListener('click', function () {
        var firstName = document.getElementById('normal-user-first-name').value;
        var lastName = document.getElementById('normal-user-last-name').value;
        var username = document.getElementById('username').value;
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");

        const userProfileUpdate = {
            Username: username,
            FirstName: firstName,
            LastName: lastName,
        };

        var modifyProfileUrl = `${baseUrl}/ModifyUserProfile/ModifyProfile`;


        fetch(modifyProfileUrl, {
            method: 'POST',
            headers: {
                'Authentication': idToken,
                'Authorization': accessToken,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userProfileUpdate),
        })
            .then(response => {
                // Check the content type of the response
                const contentType = response.headers.get("content-type");
                if (!response.ok) {
                    if (contentType && contentType.includes("application/json")) {
                        // If the response is JSON, parse it and throw an error with its content
                        return response.json().then(data => {
                            throw new Error(JSON.stringify(data) || 'HTTP error! status: ' + response.status);
                        });
                    } else {
                        // If the response is not JSON, use the text directly
                        return response.text().then(text => {
                            throw new Error(text || 'HTTP error! status: ' + response.status);
                        });
                    }
                }
                // If the response is OK, handle it according to its content type
                if (contentType && contentType.includes("application/json")) {
                    return response.json();
                } else {
                    return response.text();
                }
            })
            .then(data => {
                console.log('Profile updated successfully:', data);
                alert('Profile updated successfully: ' + data); // Adjusted to handle non-JSON responses
                fetchUserProfile(username);
            })
            .catch(error => {
                console.error('Error updating profile:', error);
            });
    });

    // Prepare UI for a root admin
    function prepareAdminUI() {
        // Show UI elements for root admin, such as user management buttons
        document.getElementById('admin-management-section').style.display = 'block';
        // Hide normal user modify section just in case it was previously shown
        document.getElementById('normal-user-modify-section').style.display = 'none';
    }

    // Inside prepareAdminUI
    document.getElementById('admin-delete-user').addEventListener('click', function () {
        var username = prompt("Enter the username of the user to delete:");
        var userName = document.getElementById('username').value;
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");

        if (username) {
            // Check if the username to delete is the admin's own username
            if (username === userName) {
                alert('Cannot delete your own admin account.');
                return; // Exit the function to prevent the deletion process
            }

            var deleteUserUrl = `${baseUrl}/ModifyUserProfile/DeleteProfile`;

            fetch(deleteUserUrl, {
                method: 'DELETE',
                headers: {
                    "Authentication": idToken,
                    "Authorization": accessToken,
                    "userName": username
                }
            })
                .then(response => {
                    if (response.ok) {
                        return response.text();
                    } else {
                        throw new Error('Failed to delete user.');
                    }
                })
                .then(data => {
                    alert('User deleted successfully.');
                    // Further actions upon successful deletion
                })
                .catch(error => {
                    console.error('Error deleting user:', error);
                });
        }
    });

    //Inside prepareAdminUI
    document.getElementById('admin-user-creation-button').addEventListener('click', function () {
        // Display the form
        var form = document.getElementById('admin-creation-form');
        form.style.display = 'block';
    });

    // Assuming you have another button to actually submit the form. Let's say it has an ID "submit-admin-form"
    document.getElementById('admin-user-creation').addEventListener('click', function () {
        var form = document.getElementById('admin-creation-form');
        var feedbackBox = document.getElementById('feedback-box');
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");

        // Check if the form is already displayed
        if (form.style.display === 'block') {
            // Form is displayed, try to submit form data
            const email = form.querySelector('#email').value;
            const dob = form.querySelector('#dob').value;
            const uname = form.querySelector('#uname').value;
            const bmail = form.querySelector('#bmail').value;

            idToken = sessionStorage.getItem("idToken");
            accessToken = sessionStorage.getItem("accessToken");
            var adminUseCreate = `${baseUrl}/AccCreationAPI/api/AdminAccCreationAPI`;

            const data = { email, dob, uname, bmail };

            fetch(adminUseCreate, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authentication': idToken,
                    'Authorization': accessToken
                },
                body: JSON.stringify(data),
            })
                .then(response => response.json())
                .then(result => {
                    feedbackBox.style.display = 'block';
                    if (result) {
                        feedbackBox.textContent = 'Admin account created successfully';
                        feedbackBox.style.color = 'green';
                        form.reset();
                    } else {
                        feedbackBox.textContent = 'Failed to create admin account';
                        feedbackBox.style.color = 'red';
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    feedbackBox.textContent = 'Error creating account. Please try again.';
                    feedbackBox.style.color = 'red';
                });
        } else {
            // Form is not displayed, so display it
            form.style.display = 'block';
        }
    });




    // Inside prepareAdminUI
    document.getElementById('admin-get-user').addEventListener('click', function () {
        var username = prompt("Enter the username of the user to fetch:");
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");
        if (username) {
            fetch(`${baseUrl}/ModifyUserProfile/AdminLookUp`, {
                method: 'GET',
                headers: {
                    'Authentication': idToken,
                    'Authorization': accessToken,
                    'userName': username
                },
            })
                .then(response => {
                    if (response.ok) {
                        return response.json();
                    } else {
                        throw new Error('Failed to fetch user details.');
                    }
                })
                .then(userProfile => {
                    // Find the display element
                    var userDetailsDisplay = document.getElementById('user-details-display');

                    // Clear previous details
                    userDetailsDisplay.innerHTML = '';

                    // Create and append new details
                    var usernameDetail = document.createElement('p');
                    usernameDetail.textContent = `Username: ${userProfile.username}`;
                    userDetailsDisplay.appendChild(usernameDetail);

                    var firstNameDetail = document.createElement('p');
                    firstNameDetail.textContent = `First Name: ${userProfile.firstName}`;
                    userDetailsDisplay.appendChild(firstNameDetail);

                    var lastNameDetail = document.createElement('p');
                    lastNameDetail.textContent = `Last Name: ${userProfile.lastName}`;
                    userDetailsDisplay.appendChild(lastNameDetail);

                    // Make the display element visible
                    userDetailsDisplay.style.display = 'block';
                })
                .catch(error => {
                    console.error('Error fetching user details:', error);
                    // Optionally, hide the display element if there was an error
                    document.getElementById('user-details-display').style.display = 'none';
                });
        }
    });

    // Inside prepareAdminUI
    document.getElementById('admin-update-claims').addEventListener('click', function () {
        var username = prompt("Enter the username of the user to update claims for:");
        var userRole = prompt("Enter the new user role for the user:");
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");

        if (username && userRole) {
            // Construct the payload according to the expected structure
            var payload = {
                Username: username,
                Claims: {
                    UserRole: userRole // Set the UserRole within the Claims object
                }
            };

            fetch(`${baseUrl}/ModifyUserProfile/updateClaims`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authentication': idToken,
                    'Authorization': accessToken
                },
                body: JSON.stringify(payload), // Use the constructed payload
            })
                .then(response => {
                    if (response.ok) {
                        return response.json();
                    } else {
                        throw new Error('Failed to update user claims.');
                    }
                })
                .then(data => {
                    alert(data.message); // Assuming the server responds with a message
                })
                .catch(error => {
                    console.error('Error updating user claims:', error);
                    alert(error.message); // Provide feedback in case of error
                });
        } else {
            alert('Username and user role are required.'); // Provide feedback if input is missing
        }
    });


    function displayUserProfile(userProfile) {
        // Dummy data for user profile, replace with real data as needed
        document.getElementById('user-last-name').textContent = userProfile.lastName || 'N/A';
        document.getElementById('user-first-name').textContent = userProfile.firstName || 'N/A';
        document.getElementById('user-email').textContent = userProfile.email || 'N/A';
        document.getElementById('user-dob').textContent = userProfile.dateOfBirth || 'N/A';
        document.getElementById('user-role').textContent = userProfile.userRole || 'N/A';
        document.getElementById('user-status').textContent = userProfile.userStatus || 'N/A';
        document.getElementById('user-description').textContent = 'A brief description here.';
        document.getElementById('user-profile-pic').src = 'profilePic.jpg'; // Update the path to the profile picture

        document.getElementById('user-profile').style.display = 'block';

        document.getElementById('modify-profile').addEventListener('click', function () {
            // Ensure the modify profile section is shown when the button is clicked
            document.getElementById('modify-profile-section').style.display = 'block';

            // Assuming fetchAndSetUserRole should also be called here
            var username = sessionStorage.getItem('username'); // Make sure this is correctly retrieving the username
            if (username) {
                fetchAndSetUserRole(username); // This will set the user role, adjust the UI as necessary
            } else {
                console.error('Username not found');
                alert('Error: Could not fetch user role. Please log in again.');
            }
        });
    }

    document.getElementById('logoutButton').addEventListener('click', function () {
        const startTime = Date.now();
        var userName = document.getElementById("username").value;
        idToken = sessionStorage.getItem("idToken");
        accessToken = sessionStorage.getItem("accessToken");
        //console.log(idToken);
        //console.log(accessToken);

        fetch(`${baseUrl}/Logout/api/logout`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authentication': idToken,
                'Authorization': accessToken,
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
                localStorage.clear()
                sessionStorage.clear()
            })
            .catch(error => {
                console.error('Error:', error);
                alert(error.message); // Display error message to the user
            });
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
        console.log(bmail);
        // Create a URLSearchParams object to encode the data
        var params = new URLSearchParams();
        params.append('email', email);
        params.append('dob', dob);
        params.append('uname', uname);
        params.append('bmail', bmail);

        // Construct the URL with the encoded parameters
        var url = `${baseUrl}/AccCreationAPI/api/NormalAccCreationAPI`;

        // Log the URL
        console.log('URL:', url);

        // Send data to your API
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                email: email, dob: dob, uname: uname, bmail:bmail
            })
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

    document.getElementById('enter-scaleDisplay').addEventListener('click', function () {
        document.querySelector('.main').style.display = 'none'; // Hide main content
        document.getElementById('tempoToolView').style.display = 'none'; // Hide tempotool view
        document.getElementById('ScaleDisplayView').style.display = 'block'; // Show tempo tool content
        var username = document.getElementById("username").value;
        logFeatureUsage(username, "Scale Display");
    });

    function logFeatureUsage(username, feature) {
        const requestData = {
            UserName: username,
            Feature: feature
        };

        fetch('http://localhost:8080/LogFeature/api/LogFeatureAPI', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authentication': idToken,
                'Authorization': accessToken
            },
            body: JSON.stringify(requestData)
        })
            .then(response => {
                if (response.ok) {
                    return response.json();
                } else {
                    throw new Error(response.json());
                }
            })
            .then(data => {
                console.log('Feature usage logged successfully');
            })
            .catch(error => {
                console.error('Error logging {feature} usage:', error.message);
            });
    }

    // Inventory Stock
    //document.getElementById('InventoryStock').addEventListener('click', function () {
    //    window.location.href = 'InventoryStockView.html'; // Redirects the user to PriceRangeSorting.html
    //});
    document.getElementById('FinancialProgressReport').addEventListener('click', function () {
        window.location.href = 'FinancialProgressReportView.html'; // Redirects the user to PriceRangeSorting.html
    });

});