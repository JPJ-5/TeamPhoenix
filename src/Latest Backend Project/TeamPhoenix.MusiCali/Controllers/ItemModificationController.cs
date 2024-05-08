using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MySqlX.XDevAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Security;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemModificationController : ControllerBase
    {
        private readonly IConfiguration configuration;
        //private readonly ApiService _apiService;
        private ItemModificationService modificationService;

        //public ItemCreationController(IConfiguration configuration, ApiService apiService)
        public ItemModificationController(IAmazonS3 s3Client, IConfiguration configuration)
        {
            this.configuration = configuration;
            modificationService = new ItemModificationService(s3Client, this.configuration);
            // _apiService = apiService;
        }


        [HttpPost("UpdateItem")]
        //public IActionResult CreateAnItem([FromBody] ItemCreationModel item, [FromHeader] string username)
        public async Task<IActionResult> UpdateItem([FromBody] ItemCreationModel item, [FromHeader] string username)
        {
            try
            {
                //try to validate, scan virus of the files
                if(string.IsNullOrWhiteSpace(username))
                {
                    throw new Exception("Invalid Userame!");
                }
                bool updateSucess = await modificationService.updateItemRequest(username, item);

                if (updateSucess)
                {
                    //HttpResponseMessage response = await client.GetAsync(uploadSandboxToS3ApiUrl);
                    return Ok($"Item {item.Sku} Sucessfully Update"); // Changed from JsonResult to IActionResult with Ok result
                }
                return BadRequest(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(false);
            }
        }
    }
}
