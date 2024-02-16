document.addEventListener('DOMContentLoaded', function () {
    var menuButton = document.getElementById('menu-btn');
    var dropdown = document.getElementById('myDropdown');
    var showLoginFormButton = document.getElementById('show-login');
    var loginForm = document.getElementById('login-form');
    var showOtpButton = document.getElementById('show-otp');
    var otpForm = document.getElementById('otp-form');
    var showRegisterButton = document.getElementById('show-register');
    var registerEmailForm = document.getElementById('register-email-form');
    var showDetailsFormButton = document.getElementById('show-details-form');
    var registerDetailsForm = document.getElementById('register-details-form');
    var showRecoveryButton = document.getElementById('account-recovery-button');
    var email = ''
    var emailnotfound = document.getElementById('emailnotfounderror')
    var invalidOTP = document.getElementById('invalidotperror')


    menuButton.addEventListener('click', function () {
        dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
        // Reset the visibility of login and register buttons when menu is toggled
        resetButtonVisibility();
    });

    //console.log('success :DD')

    loginForm.addEventListener('submit', function(event){
        console.log(event)
        event.preventDefault()
        const data = new FormData(event.target);
        var payload = Object.fromEntries(data.entries())
        email = payload.email
        axios.post('/login/requestotp', payload).then(function(response){
            emailnotfound.style.display = 'none'
            //handle login here
        })
        .catch(function (){
            emailnotfound.style.display = 'block'
        })
        //move to successful login when implemented
        hideGroup('group2');
        hideGroup('group3');
        otpForm.style.display = 'block';
    })

    showLoginFormButton.addEventListener('click', function () {
        hideGroup('group2');
        hideGroup('group3');
        loginForm.style.display = 'block';
        showRegisterButton.style.display = 'none'; // Hide the register button
        showRecoveryButton.style.display = 'none';
    });
/*
    showOtpButton.addEventListener('click', function () {
        hideGroup('group2');
        hideGroup('group3');
        otpForm.style.display = 'block';
    });*/

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

    // Add event listener for OTP form submission
    document.getElementById('otp-form').addEventListener('submit', function (event) {
        event.preventDefault();
        const data = new FormData(event.target);
        var payload = Object.fromEntries(data.entries())
        payload.email = email
        axios.post('/login/otpverify', payload).then(function(response){
            invalidOTP.style.display = 'none'
            //handle login here
        })
        .catch(function (){
            invalidOTP.style.display = 'block'
        })
        //move to successful login when implemented
        // Hide forms and display user profile
        hideGroup('group1');
        hideGroup('group2');
        hideGroup('group3');
        displayUserProfile(); // Function to populate and show the user profile

    });

    document.getElementById('account-recovery-button').addEventListener('click', function () {
        document.getElementById('account-recovery-section').style.display = 'block';
    });

    // Event listener for submit-recovery-email
    document.getElementById('submit-recovery-email').addEventListener('click', function () {
        var email = document.getElementById('recovery-email').value;
        // Implement logic to verify if the email matches a disabled account
        // If matched, show the OTP section
        document.getElementById('otp-recovery-section').style.display = 'block';
    });
  

    // Event listener for submit-recovery-otp
    document.getElementById('submit-recovery-otp').addEventListener('click', function () {
        var otp = document.getElementById('recovery-otp').value;
        // Implement logic to verify the OTP
        // If OTP is correct, notify the user about reactivation process
        alert('OTP verified. Your account will be reviewed for reactivation by an admin.');
    });

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
        document.getElementById('editBtn').style.display = 'block';
        var userRole = document.getElementById('user-role').textContent;
        if (userRole === 'rootAdmin') {
            document.getElementById('create-admin-button').style.display = 'block';
            document.getElementById('modify-account-button').style.display = 'block';
        }
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
});
