using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CollabSearchController : ControllerBase
    {
        private readonly CollabSearchService _collabSearchService;

        public CollabSearchController(CollabSearchService collabSearchService)
        {
            _collabSearchService = collabSearchService;
        }

        [HttpPost("api/searchAPI")]
        public async Task<IActionResult> SearchUsers([FromBody] SearchModel model)
        {
            var users = await _collabSearchService.SearchUsersAsync(model.Username);
            return Ok(users);
        }

        [HttpPost("api/visibilityAPI")]
        public async Task<IActionResult> UpdateUserVisibility([FromBody] UserVisibilityModel model)
        {
            await _collabSearchService.UpdateUserVisibilityAsync(model.Username, model.Visibility);
            return Ok("Visibility updated successfully");
        }

    }
}
public class UserVisibilityModel
{
    public string Username { get; set; }
    public string Visibility { get; set; }
}
public class SearchModel
{
    public string Username { get; set; }
}
