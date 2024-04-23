using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Security;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemCreationController : Controller
    {
        private readonly IConfiguration configuration;
        public ItemCreationController(IConfiguration configuration)
        {
            this.configuration = configuration;
            
        }


        [HttpPost("api/ItemCreation")]
        public IActionResult CreateAnItem([FromBody] ItemCreationModel item)
        {
            ItemCreationService uC = new ItemCreationService(configuration);
            if (uC.CreateAnItem(item.Email, item.Dob, item.Uname, item.Bmail))
            {
                return Ok(true); // Changed from JsonResult to IActionResult with Ok result
            }
            return BadRequest(false); // Changed from JsonResult to IActionResult with Ok result
        }
    }
}
