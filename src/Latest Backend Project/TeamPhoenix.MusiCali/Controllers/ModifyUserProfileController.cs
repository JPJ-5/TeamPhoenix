using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using modifyUserService = TeamPhoenix.MusiCali.DataAccessLayer.ModifyUser;
using DataAccessUserDeletion = TeamPhoenix.MusiCali.DataAccessLayer.UserDeletion; // Alias for clarity
using mU = TeamPhoenix.MusiCali.DataAccessLayer.ModifyUser;
using wowC = TeamPhoenix.MusiCali.Security.Authentication;
using Org.BouncyCastle.Asn1.Ocsp;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModifyUserProfileController : ControllerBase
    {
        [HttpGet("AdminLookUp")]
        public IActionResult GetProfile([FromHeader]string username)
        {
            var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var role = wowC.getScopeFromToken(accessToken!);

            var user = wowC.getUserFromToken(accessToken!);


            if ((role != string.Empty) && wowC.CheckIdRoleExisting(user, role))
            {
                var modifyUserService = new modifyUserService(); // Create an instance of ModifyUser
                var userProfile = modifyUserService.GetProfile(username); // Now you can call the instance method
                if (userProfile == null)
                {
                    return NotFound("User profile not found.");
                }
                return Ok(userProfile);


            }
            else
            {
                return BadRequest("Unauthenticated!");
            }

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



            var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var role = wowC.getScopeFromToken(accessToken!);

            var user = wowC.getUserFromToken(accessToken!);

            if ((role != string.Empty) && wowC.CheckIdRoleExisting(user, role))
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
            else
            {
                return BadRequest("Unauthenticated!");
            }

            
        }



        [HttpDelete("DeleteProfile")]
        public IActionResult DeleteUser([FromHeader]string username)
        {

            var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var role = wowC.getScopeFromToken(accessToken!);

            var user = wowC.getUserFromToken(accessToken!);

            if ((role != string.Empty) && wowC.CheckIdRoleExisting(user, role))
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
            else
            {
                return BadRequest("Unauthenticated!");
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

                var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                var role = wowC.getScopeFromToken(accessToken!);



                if ((role != string.Empty) && wowC.CheckIdRoleExisting(model.Username, role))
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
                else
                {
                    return BadRequest("Unauthenticated!");
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetUserInformation")]
        public IActionResult GetUserInformation([FromHeader]string username)
        {
            //Console.WriteLine("HEREEEEE");
            try
            {

                var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                var role = wowC.getScopeFromToken(accessToken!);



                if ((role != string.Empty) && wowC.CheckIdRoleExisting(username, role))
                {
                    var modifyUserService = new mU(); // Assuming ModifyUser is in the TeamPhoenix.MusiCali.DataAccessLayer namespace
                    var userInformation = modifyUserService.GetUserInformation(username);
                    //Console.WriteLine(userInformation);

                    if (userInformation != null)
                    {
                        return Ok(userInformation);
                    }
                    else
                    {
                        return NotFound("User information not found.");
                    }
                }
                else
                {
                    return BadRequest("Unauthenticated!");
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
