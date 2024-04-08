using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BingoBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
