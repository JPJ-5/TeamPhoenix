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
        public async Task<IActionResult> UploadFile([FromBody] FileUploadViewModel model)
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
        public IActionResult DeleteFile([FromBody] string username, int slot)
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
        public IActionResult LoadArtistProfile([FromQuery] string username)
        {
            try
            {
                var artistInfo = ArtistPortfolioDao.GetProfileInfo(username);
                var fileInfo = ArtistPortfolioDao.GetAllFileInfo(username);
                if (artistInfo == null)
                {
                    return NotFound("Artist Info not found.");
                }
                if (fileInfo == null)
                {
                    return NotFound("Artist Info not found.");
                }
                var filePaths = fileInfo[0];
                var localFilePaths = ArtistPortfolio.DownloadFilesLocally(filePaths);
                if (localFilePaths == null)
                {
                    return NotFound("local files unable to be saved locally not found.");
                }
                var genreList = fileInfo[1];
                var descList = fileInfo[2];


                // Prepare the response object
                var responseData = new ArtistProfileViewModel
                {
                    Occupation = artistInfo[0],
                    Bio = artistInfo[1],
                    Location = artistInfo[2],
                    File0Path = localFilePaths[0],
                    File1Path = localFilePaths[1],
                    File1Genre = genreList[0],
                    File1Desc = descList[0],
                    File2Path = localFilePaths[2],
                    File2Genre = genreList[1],
                    File2Desc = descList[1],
                    File3Path = localFilePaths[3],
                    File3Genre = genreList[2],
                    File3Desc = descList[2],
                    File4Path = localFilePaths[4],
                    File4Genre = genreList[3],
                    File4Desc = descList[3],
                    File5Path = localFilePaths[5],
                    File5Genre = genreList[4],
                    File5Desc = descList[4],
                };

                return Ok(responseData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading artist profile: {ex.Message}");
            }
        }



        [HttpPost("api/deleteLocalFilesApi")]
        public IActionResult DeleteLocalFiles([FromBody] List<string> filePaths)
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