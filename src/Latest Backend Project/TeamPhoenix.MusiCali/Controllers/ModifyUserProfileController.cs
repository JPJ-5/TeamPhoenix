using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using modifyUserService = TeamPhoenix.MusiCali.DataAccessLayer.ModifyUser;
using DataAccessUserDeletion = TeamPhoenix.MusiCali.DataAccessLayer.UserDeletion; // Alias for clarity
using mU = TeamPhoenix.MusiCali.DataAccessLayer.ModifyUser;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModifyUserProfileController : ControllerBase
    {
        [HttpGet("{username}")]
        public IActionResult GetProfile(string username)
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
            public string Username { get; set; } = string.Empty;
            public UserClaims Claims { get; set; } = new UserClaims();
        }

        public class UserClaims
        {
            public string UserRole { get; set; } = string.Empty;
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
            public string Username { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
        }

        [HttpPost("ModifyProfile")]
        public IActionResult ModifyProfile([FromBody] UserProfileUpdateModel model)
        {
            try
            {
                DataAccessLayer.ModifyUser modifyUser = new mU();

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

        [HttpGet("GetUserInformation/{username}")]
        public IActionResult GetUserInformation(string username)
        {
            try
            {
                var modifyUserService = new mU(); // Assuming ModifyUser is in the TeamPhoenix.MusiCali.DataAccessLayer namespace
                var userInformation = modifyUserService.GetUserInformation(username);
                Console.WriteLine(userInformation);

                if (userInformation != null)
                {
                    return Ok(userInformation);
                }
                else
                {
                    return NotFound("User information not found.");
                }
            }
            catch (KeyNotFoundException knf)
            {
                // If username not found
                return NotFound(knf.Message);
            }
            catch (Exception ex)
            {
                // For other exceptions, consider logging the exception details
                return StatusCode(500, $"An error occurred while retrieving the user information: {ex.Message}");
            }
        }
    }
}
