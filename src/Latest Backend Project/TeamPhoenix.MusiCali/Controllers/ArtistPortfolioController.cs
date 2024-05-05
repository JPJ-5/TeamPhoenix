using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistPortfolioController : ControllerBase
    {
        private readonly IConfiguration? config;
        private readonly ArtistPortfolio artistPortfolio;
        private readonly ArtistPortfolioDao artistPortfolioDao;

        public ArtistPortfolioController(IConfiguration _config){
            config = _config;
            artistPortfolioDao = new ArtistPortfolioDao(_config);
            artistPortfolio = new ArtistPortfolio(config);
        }


        [HttpGet("api/loadApi")]
        public IActionResult LoadProfile([FromHeader] string Username)
        {
            try
            {
                ArtistProfileViewModel artistProfileViewModel = artistPortfolio.LoadArtistProfile(Username);
                return Ok(artistProfileViewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading artist profile: {ex.Message}");
            }
        }


        [HttpPost("api/uploadApi")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadViewModel model)
        {
            try
            {
                var genre = model.Genre ?? "N/A";
                var desc = model.Desc ?? "N/A";
                // Save the file and other information to the database
                var result = await artistPortfolio.UploadFile(model.Username, model.Slot, model.File, model.Genre, model.Desc);
                if (result.Success)
                {
                    return Ok("File uploaded successfully.");
                }
                else
                {
                    return StatusCode(500, $"Failed to upload file: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }

        [HttpPost("api/updateInfoApi")]
        public IActionResult UpdateInfo([FromBody] List<string> sectionInfo)
        {
            try
            {
                var username = sectionInfo[0];
                var section = sectionInfo[1];
                var info = sectionInfo[2];
                // Save the information to the database
                var result = artistPortfolioDao.updateInfo(username, section, info);
                if (result.Success)
                {
                    return Ok("Info uploaded successfully.");
                }
                else
                {
                    return BadRequest($"Failed to upload info: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating portfolio info: {ex.Message}");
            }
        }

        [HttpPost("api/delInfoApi")]
        public IActionResult DeleteInfo([FromBody] List<string> sectionRequest)
        {
            try
            {
                var username = sectionRequest[0];
                var section = sectionRequest[1];
                // Save the information to the database
                var result = artistPortfolioDao.DeleteSection(username, section);
                if (result.Success)
                {
                    return Ok("Section deleted successfully.");
                }
                else
                {
                    return BadRequest($"Failed to delete section info: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting portfolio info: {ex.Message}");
            }
        }

        [HttpPost("api/deleteApi")]
        public IActionResult DeleteFile([FromForm] DeleteFileRequest req)
        {
            string? user = req.Username;
            int? fileSlot = req.SlotNumber;
            try
            {
                var result = artistPortfolio.DeleteFile(user, fileSlot);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting portfolio File: {ex.Message}");
            }
        }

    }
}
