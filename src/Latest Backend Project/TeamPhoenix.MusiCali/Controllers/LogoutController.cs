using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Services;
using System.Threading.Tasks;
using Newtonsoft.Json;

[ApiController]
[Route("[controller]")]
public class LogoutController : ControllerBase
{
    private readonly LogoutService _logoutService;

    public LogoutController(LogoutService logoutService)
    {
        _logoutService = logoutService;
    }

    public class LogoutRequest
    {
        public string UserName { get; set; } = string.Empty;
    }


    [HttpPost("api/logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.UserName))
        {
            return BadRequest(new { message = "User hash is required" });
        }

        var result = await _logoutService.LogoutUserAsync(request.UserName);
        if (result)
        {
            return Ok(new { message = "Logout successful and logged" });
        }
        else
        {
            return BadRequest(new { message = "Logout failed" });
        }
    }
}
