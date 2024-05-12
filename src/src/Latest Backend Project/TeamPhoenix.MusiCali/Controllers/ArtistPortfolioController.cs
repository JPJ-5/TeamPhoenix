using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;
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
        private LoggerService loggerService;

        public ArtistPortfolioController(IConfiguration _config){
            config = _config;
            artistPortfolioDao = new ArtistPortfolioDao(_config);
            artistPortfolio = new ArtistPortfolio(_config);
            loggerService = new LoggerService(_config);
        }


        [HttpGet("api/loadApi")]
        public IActionResult LoadProfile([FromHeader] string Username)
        {
            try
            {
                ArtistProfileViewModel artistProfileViewModel = artistPortfolio.LoadArtistProfile(Username);
                loggerService.LogSuccessFailure(Username, "Info", "View", $"ArtistPortfolio,  View successfully loaded");
                return Ok(artistProfileViewModel);
            }
            catch (Exception ex)
            {
                loggerService.LogSuccessFailure(Username, "Error", "Data", "ArtistPortfolio, Profile was not able to be loaded");
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
                var result = await artistPortfolio.UploadFile(model.Username, model.Slot, model.File!, model.Genre, model.Desc);
                if (result.Success)
                {
                    loggerService.LogSuccessFailure(model.Username!, "Info", "Data", $"ArtistPortfolio, File successfully uploaded");
                    return Ok("File uploaded successfully.");
                }
                else
                {
                    loggerService.LogSuccessFailure(model.Username!, "Error", "Data", "ArtistPortfolio, File was not able to be uploaded");
                    return StatusCode(500, $"Failed to upload file: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                loggerService.LogSuccessFailure(model.Username!, "Error", "Data", "ArtistPortfolio, Fileile was not able to be uploaded");
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
                    loggerService.LogSuccessFailure(username, "Info", "Data", $"ArtistPortfolio, Info section successfully updated");
                    return Ok("Info uploaded successfully.");
                }
                else
                {
                    loggerService.LogSuccessFailure(username, "Error", "Data", "ArtistPortfolio, Artist info was not able to be uploaded");
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
            var username = sectionRequest[0];
            var section = sectionRequest[1];
            try
            {
                // Save the information to the database
                var result = artistPortfolioDao.DeleteSection(username, section);
                if (result.Success)
                {
                    loggerService.LogSuccessFailure(username, "Info", "Data", $"ArtistPortfolio, Info section successfully deleted");
                    return Ok("Section deleted successfully.");
                }
                else
                {
                    loggerService.LogSuccessFailure(username, "Error", "Data", "ArtistPortfolio, Info was not able to be deleted");
                    return BadRequest($"Failed to delete section info: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                loggerService.LogSuccessFailure(username, "Error", "Data", "ArtistPortfolio, Info was not able to be deleted");
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
                loggerService.LogSuccessFailure(req.Username!, "Info", "Data", $"ArtistPortfolio, File successfully deleted");
                return Ok(result);
            }
            catch (Exception ex)
            {
                loggerService.LogSuccessFailure(user!, "Error", "Data", "ArtistPortfolio, File was not able to be deleted");
                return StatusCode(500, $"Error deleting portfolio File: {ex.Message}");
            }
        }

        [HttpPost("api/updateVisibility")]
        public IActionResult UpdateVis([FromBody] PortfolioVisibility vis)
        {
            try
            {
                var result = artistPortfolioDao.updateVisibility(vis.Username!, vis.Visibility);
                loggerService.LogSuccessFailure(vis.Username!, "Info", "Business", $"ArtistPortfolio, Portfolio updated to {vis.Visibility}");
                return Ok(result);
            } 
            catch (Exception ex)
            {
                loggerService.LogSuccessFailure(vis.Username!, "Error", "Data", "ArtistPortfolio, Unable to update user visibility");
                return StatusCode(500, $"Error updating visibility: {ex.Message}");
            }
        }
    }
}
