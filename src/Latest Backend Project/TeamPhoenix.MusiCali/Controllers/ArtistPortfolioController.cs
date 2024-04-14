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
        [HttpPost("api/loadApi")]
        public IActionResult LoadArtistProfile([FromBody] string username)
        {
            try
            {
                var file = ArtistPortfolioDao.GetPortfolio(username);
                var fileInfo = file[0];
                var localFiles = ArtistPortfolio.DownloadFilesLocally(fileInfo);
                var genreList = file[1];
                var descList = file[2];
                var artistInfo =file[3];
                if (artistInfo == null || fileInfo == null)
                {
                    return NotFound("Artist info or file info not found.");
                }

                var responseData = new ArtistProfileViewModel
                {
                    Occupation = artistInfo[0],
                    Bio = artistInfo[1],
                    Location = artistInfo[2],
                    File0 = GetFileBase64(localFiles[0]),
                    File1 = GetFileBase64(localFiles[1]),
                    File2 = GetFileBase64(localFiles[2]),
                    File3 = GetFileBase64(localFiles[3]),
                    File4 = GetFileBase64(localFiles[4]),
                    File5 = GetFileBase64(localFiles[5]),
                    File0Ext = Path.GetExtension(localFiles[0]),
                    File1Ext = Path.GetExtension(localFiles[1]),
                    File2Ext = Path.GetExtension(localFiles[2]),
                    File3Ext = Path.GetExtension(localFiles[3]),
                    File4Ext = Path.GetExtension(localFiles[4]),
                    File5Ext = Path.GetExtension(localFiles[5]),
                    File1Genre = genreList[0],
                    File2Genre = genreList[1],
                    File3Genre = genreList[2],
                    File4Genre = genreList[3],
                    File5Genre = genreList[4],
                    File1Desc = descList[0],
                    File2Desc = descList[1],
                    File3Desc = descList[2],
                    File4Desc = descList[3],
                    File5Desc = descList[4],
                };
                foreach (string path in localFiles) {
                    if (System.IO.File.Exists(path) && path is not null) {
                        System.IO.File.Delete(path);
                    }
                }

                return Ok(responseData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading artist profile: {ex.Message}");
            }
        }

        private string GetFileBase64(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return Convert.ToBase64String(fileBytes);
                }
                else
                {
                    Console.WriteLine($"File '{filePath}' does not exist.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file '{filePath}': {ex.Message}");
                return null;
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
                var result = await ArtistPortfolio.UploadFile(model.Username, model.Slot, model.File, model.Genre, model.Desc);
                if (result.Success)
                {
                    return Ok("File uploaded successfully.");
                }
                else
                {
                    return BadRequest("Failed to upload file.");
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
                var result = ArtistPortfolioDao.updateInfo(username, section, info);
                if (result.Success)
                {
                    return Ok("File uploaded successfully.");
                }
                else
                {
                    return BadRequest("Failed to upload file.");
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
                var result = ArtistPortfolioDao.DeleteSection(username, section);
                if (result.Success)
                {
                    return Ok("Section deleted successfully.");
                }
                else
                {
                    return BadRequest("Failed to delete section info.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating portfolio info: {ex.Message}");
            }
        }

        [HttpPost("api/deleteApi")]
        public IActionResult DeleteFile([FromForm] DeleteFileRequest req)
        {
            string user = req.Username;
            int fileSlot = req.SlotNumber;
            try
            {
                var result = ArtistPortfolio.DeleteFile(user, fileSlot);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting file: ", ex);
            }
        }

    }
}
