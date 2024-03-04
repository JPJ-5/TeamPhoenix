using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using modifyUserService = TeamPhoenix.MusiCali.DataAccessLayer.ModifyUser;
using DataAccessUserDeletion = TeamPhoenix.MusiCali.DataAccessLayer.UserDeletion; // Alias for clarity
using cr = TeamPhoenix.MusiCali.DataAccessLayer.UserCreation;
using mU = TeamPhoenix.MusiCali.DataAccessLayer.ModifyUser;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModifyUserProfileController : ControllerBase
    {
        [HttpGet("{username}")]
        public ActionResult<UserProfile> GetProfile(string username)
        {
            var modifyUserService = new modifyUserService(); // Create an instance of ModifyUser
            var userProfile = modifyUserService.GetProfile(username); // Now you can call the instance method
            if (userProfile == null)
            {
                return NotFound("User profile not found.");
            }
            return Ok(userProfile);
        }

        public class UpdateClaimsRequest
        {
            public string Username { get; set; }
            public UserClaims Claims { get; set; }
        }

        public class UserClaims
        {
            public string UserRole { get; set; }
        }

        [HttpPost("updateClaims")]
        public IActionResult UpdateClaims([FromBody] UpdateClaimsRequest request)
        {
            // Assuming mU is an alias for your ModifyUser class
            var modifyUserService = new mU(); // Create an instance of ModifyUser
            bool success = modifyUserService.UpdateClaims(request.Username, new Dictionary<string, string> { { "UserRole", request.Claims.UserRole } });

            if (success)
            {
                return Ok(new { success = true, message = "Claims updated successfully, user promoted to admin." });
            }
            else
            {
                return BadRequest(new { success = false, message = "Failed to update claims." });
            }
        }



        [HttpDelete("{username}")]
        public IActionResult DeleteUser(string username)
        {
            // Call the DeleteProfile method from UserDeletion class
            if (DataAccessUserDeletion.DeleteProfile(username))
            {
                return Ok("User profile deleted successfully.");
            }
            else
            {
                return BadRequest("Failed to delete user profile.");
            }
        }

        public class UserProfileUpdateModel
        {
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        [HttpPost("ModifyProfile")]
        public IActionResult ModifyProfile([FromBody] UserProfileUpdateModel model)
        {
            try
            {
                DataAccessLayer.ModifyUser modifyUser = new DataAccessLayer.ModifyUser();

                // Call ModifyProfile method to update the user profile using the model properties
                bool success = modifyUser.ModifyProfile(model.Username, model.FirstName, model.LastName);

                if (success)
                {
                    return Ok("User profile updated successfully.");
                }
                else
                {
                    return StatusCode(500, "Failed to update user profile.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetDOB/{username}")]
        public ActionResult<string> GetProfileDOB(string username)
        {
            try
            {
                var modifyUserService = new DataAccessLayer.ModifyUser(); // Assuming ModifyUser is in the TeamPhoenix.MusiCali.DataAccessLayer namespace
                DateTime dob = modifyUserService.GetProfileDOB(username);
                // Format the DateTime to a string with just the date part
                string formattedDob = dob.ToString("yyyy-MM-dd");
                return Ok(formattedDob);
            }
            catch (KeyNotFoundException knf)
            {
                // If username not found
                return NotFound(knf.Message);
            }
            catch (Exception ex)
            {
                // For other exceptions, consider logging the exception details
                return StatusCode(500, $"An error occurred while retrieving the DOB: {ex.Message}");
            }
        }

    }
}
