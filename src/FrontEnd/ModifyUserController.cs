using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using modifyUserService = TeamPhoenix.MusiCali.DataAccessLayer.ModifyUser;
using DataAccessUserDeletion = TeamPhoenix.MusiCali.DataAccessLayer.UserDeletion; // Alias for clarity
using cr = TeamPhoenix.MusiCali.DataAccessLayer.UserCreation;

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

        [HttpPost("updateClaims")]
        public IActionResult UpdateClaims([FromBody] UserClaims claims, [FromQuery] string username)
        {
            var modifyUserService = new modifyUserService(); // Create an instance of ModifyUser
            bool success = modifyUserService.UpdateClaims(username, claims.Claims); // Call the instance method
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

        [HttpPost("ModifyProfile")]
        public IActionResult ModifyProfile(string username, string firstName, string lastName)
        {
            try
            {
                DataAccessLayer.ModifyUser modifyUser = new DataAccessLayer.ModifyUser();

                // Call ModifyProfile method to update the user profile
                bool success = modifyUser.ModifyProfile(username, firstName, lastName);

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
    }
}
