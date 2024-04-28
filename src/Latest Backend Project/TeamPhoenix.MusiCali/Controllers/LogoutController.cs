using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Services;
using TeamPhoenix.MusiCali.Security;
using TeamPhoenix.MusiCali.DataAccessLayer;

[ApiController]
[Route("[controller]")]
public class LogoutController : ControllerBase
{
    private readonly LogoutService logoutService;
    private readonly IConfiguration configuration;
    private AuthenticationSecurity authenticationSecurity;

    public LogoutController(IConfiguration configuration)
    {
        this.configuration = configuration;
        logoutService = new LogoutService(this.configuration);
        authenticationSecurity = new AuthenticationSecurity(this.configuration);
    }

    public class LogoutRequest
    {
        public string UserName { get; set; } = string.Empty;
    }


    [HttpPost("api/logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var role = authenticationSecurity.getScopeFromToken(accessToken!);


        if ((role != string.Empty) && authenticationSecurity.CheckIdRoleExisting(request.UserName, role))
        {
            
            if (request == null || string.IsNullOrEmpty(request.UserName))
            {
                return BadRequest(new { message = "User hash is required" });
            }

            var result = await logoutService.LogoutUserAsync(request.UserName);
            if (result)
            {
                // Call to deauthenticate the user
                var deauthResult = new AuthenticationDAO(configuration).DeauthenticateUser(request.UserName);
                if (deauthResult)
                {
                    return Ok(new { message = "Logout successful and user deauthenticated" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to deauthenticate user" });
                }
            }
            else
            {
                return BadRequest(new { message = "Logout failed" });
            }
            
        }
        else
        {
            return BadRequest("Unauthenticated!");
        }
    }
}
