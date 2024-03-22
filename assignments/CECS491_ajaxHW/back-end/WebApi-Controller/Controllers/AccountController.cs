using System; //used for random
using LibAccount;
using Microsoft.AspNetCore.Mvc; // Namespace needed for using Controllers

namespace WebApi_Controller.Controllers;

[ApiController]
[Route("[controller]")]  // Defines the default parent URL path for all action methods to be the name of controller
public class AccountController : ControllerBase
{

    private readonly AuthChecker _authChecker;

    public AccountController(AuthChecker authChecker) // .NET's DI will inject the proper instance based on DI registration in Program.cs
    {
        _authChecker = authChecker;
    }

    [HttpGet] // HTTP GET for development, but consider HTTP HEAD for production
    [Route("health")]  // Endpoint route:  /account/health
    public Task<IActionResult> HealthCheck()
    {
        return Task.FromResult<IActionResult>(Ok("Healthy")); // NoContent()
    }


    [Route("data")] // Endpoint route:  /account/data
    public Task<IActionResult> Get() // Using convention routing to make action method reachable with HTTP GET
    {
        Random rnd = new Random();
        var randomSize = rnd.Next(10);
        string[] nameList = {"Edith", "Jason", "Iggy", "Paula", "Sera", "Jeff", "Tony", "Angela", "Nico", "Yuko"};
        string[] data = new string[randomSize];
        for (int i = 0; i < randomSize; i++)
        {
            data[i] = nameList[rnd.Next(10)];
        }

        return Task.FromResult<IActionResult>(Ok(data));
    }


    [HttpPost]
    [Route("data")] // Endpoint route:  /account/data
    public Task<IActionResult> Create()
    {
        /* Porting code from Console app to Web app is trivial when proper design encapsulation is performed

            var authDAO = new AuthDAO();
            var authChecker = new AuthChecker(authDAO);
            var accountManager = new AccountManager(authChecker);

            var result = accountManager.CreateAccount();

            if(result)
            {
                Console.WriteLine("Account created")  // Is this proper for a web app?
            }
          
        */

        // Use the following cURL command to send a POST request to this action method
        // curl -i -X POST "https://localhost:7079/account/data"

        return Task.FromResult<IActionResult>(Ok("Account created"));
    }

}

