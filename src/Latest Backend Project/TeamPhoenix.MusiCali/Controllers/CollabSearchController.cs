using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollabSearchController : ControllerBase
    {
        private readonly CollabSearchService _collabSearchService;

        public CollabSearchController(CollabSearchService collabSearchService)
        {
            _collabSearchService = collabSearchService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers(string username)
        {
            var users = await _collabSearchService.SearchUsersAsync(username);
            return Ok(users);
        }

        [HttpPost("visibility")]
        public async Task<IActionResult> UpdateUserVisibility(string username, string visibility)
        {
            await _collabSearchService.UpdateUserVisibilityAsync(username, visibility);
            return Ok("Visibility updated successfully");
        }
    }
}
