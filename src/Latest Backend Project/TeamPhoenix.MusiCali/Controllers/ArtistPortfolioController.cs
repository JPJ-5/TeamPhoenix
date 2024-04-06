using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.Services;
using System;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.DataAccessLayer;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistPortfolioController : ControllerBase
    {

        [HttpPost("api/uploadApi")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadViewModel model)
        {
            try
            {
                var result = await ArtistPortfolio.UploadFile(model.Username, model.Slot, model.File, model.Genre, model.Desc);
                if (result.Success)
                {
                    return Ok("File uploaded successfully.");
                }
                else
                {
                    return StatusCode(500, $"Error uploading file: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }

        [HttpDelete("api/deleteApi")]
        public IActionResult DeleteFile(string username, int slot)
        {
            try
            {
                var result = ArtistPortfolio.DeleteFile(username, slot);
                if (result.Success)
                {
                    return Ok("File deleted successfully.");
                }
                else
                {
                    return StatusCode(500, $"Error deleting file: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting file: {ex.Message}");
            }
        }

        [HttpGet("api/loadApi")]
        public IActionResult LoadArtistProfile(string username)
        {
            try
            {
                var artistInfo = ArtistPortfolioDao.GetProfileInfo(username);
                var fileInfo = ArtistPortfolioDao.GetAllFileInfo(username);
                var filePaths = fileInfo[0];
                var localFilePaths = ArtistPortfolio.DownloadFilesLocally(filePaths);
                List<List<string>> localFileInfo = new List<List<string>>();
                localFileInfo.Add(localFilePaths);
                localFileInfo.Add(fileInfo[1]);
                localFileInfo.Add(fileInfo[1]);


                // Prepare ViewModel and send it to frontend
                var viewModel = new ArtistProfileViewModel
                {
                    ProfileInfo = artistInfo,
                    LocalFileInfo = localFileInfo
                };

                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading artist profile: {ex.Message}");
            }
        }

        [HttpPost("api/deleteLocalFilesApi")]
        public IActionResult DeleteLocalFiles(List<string> filePaths)
        {
            try
            {
                ArtistPortfolio.DeleteLocalFiles(filePaths);
                return Ok("Local files deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting local files: {ex.Message}");
            }
        }
    }
}
