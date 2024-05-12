using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;
using TeamPhoenix.MusiCali.Security;
using TeamPhoenix.MusiCali.Services;

[ApiController]
[Route("SellerDashboard")]
public class FinancialProgressReportController : ControllerBase
{
    private FinancialProgressReportService _financialService;
    private AuthenticationSecurity _authentication;

    public FinancialProgressReportController(IConfiguration configuration)
    {
        _financialService = new FinancialProgressReportService(configuration);
        _authentication = new AuthenticationSecurity(configuration);
    }

    [HttpGet("api/GetFinancialReport")]
    public IActionResult GetInventoryStock([FromHeader] string username, [FromHeader] string frequency)
    {
        try
        {
            var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var role = _authentication.getScopeFromToken(accessToken!);
            var user = _authentication.getUserFromToken(accessToken!);

            if (!string.IsNullOrEmpty(role) && _authentication.CheckIdRoleExisting(user, role))
            {
                var report = _financialService.GetReport(username, frequency);
                if (report != (new HashSet<FinancialInfoModel>()))
                {
                    return Ok(report);
                }
                else
                {
                    throw new Exception("Invalid Report. Please Try Again Or Contact The System Administrator.");
                }
            }
            else
            {
                throw new Exception("Invalid Identity. Please Contact The System Administrator.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest($"Internal server error: {ex.Message}");
        }
    }
}