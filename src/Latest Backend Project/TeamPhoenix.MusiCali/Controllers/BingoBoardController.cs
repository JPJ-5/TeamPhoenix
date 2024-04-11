using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using BB = TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.Services.BingoBoard;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BingoBoardController : Controller
    {
        public IActionResult ViewMultipleGigs()
        {
            return View();
        }
    }
}
