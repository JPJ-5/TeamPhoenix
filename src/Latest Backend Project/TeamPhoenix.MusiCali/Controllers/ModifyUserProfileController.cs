using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.Security;
using TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModifyUserProfileController : ControllerBase
    {
        private AuthenticationSecurity authentication;
        private ModifyUserService modifyUserService;
        private UserDeletionDAO userDeletionService;
        private ModifyUserDAO modifyUserDAO;
        private readonly IConfiguration configuration;
        public ModifyUserProfileController(IConfiguration configuration)
        {
            this.configuration = configuration;
            authentication = new AuthenticationSecurity(configuration);
            modifyUserService = new ModifyUserService(configuration);
            modifyUserDAO = new ModifyUserDAO(configuration);
            userDeletionService = new UserDeletionDAO(configuration);
        }

        [HttpGet("AdminLookUp")]
        public IActionResult GetProfile([FromHeader] string username)
        {
            var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var role = authentication.getScopeFromToken(accessToken!);
            var user = authentication.getUserFromToken(accessToken!);

            if (!string.IsNullOrEmpty(role) && authentication.CheckIdRoleExisting(user, role))
            {
                var userProfile = modifyUserDAO.GetProfile(username);
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

        [HttpPost("updateClaims")]
        public IActionResult UpdateClaims([FromBody] UpdateClaimsRequest request)
        {
            var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var role = authentication.getScopeFromToken(accessToken!);
            var user = authentication.getUserFromToken(accessToken!);

            if (!string.IsNullOrEmpty(role) && authentication.CheckIdRoleExisting(user, role))
            {
                bool success = modifyUserDAO.UpdateClaims(request.Username, new Dictionary<string, string> { { "UserRole", request.Claims.UserRole } });
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
        public IActionResult DeleteUser([FromHeader] string username)
        {
            var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var role = authentication.getScopeFromToken(accessToken!);
            var user = authentication.getUserFromToken(accessToken!);

            if (!string.IsNullOrEmpty(role) && authentication.CheckIdRoleExisting(user, role))
            {
                if (userDeletionService.DeleteProfile(username))
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

        [HttpPost("ModifyProfile")]
        public IActionResult ModifyProfile([FromBody] UserProfileUpdateModel model)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var role = authentication.getScopeFromToken(accessToken!);

                if (!string.IsNullOrEmpty(role) && authentication.CheckIdRoleExisting(model.Username, role))
                {
                    bool success = modifyUserDAO.ModifyProfile(model.Username, model.FirstName, model.LastName);

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
        public IActionResult GetUserInformation([FromHeader] string username)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var role = authentication.getScopeFromToken(accessToken!);

                if (!string.IsNullOrEmpty(role) && authentication.CheckIdRoleExisting(username, role))
                {
                    var userInformation = modifyUserDAO.GetUserInformation(username);

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
                return NotFound(knf.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the user information: {ex.Message}");
            }
        }
    }
}