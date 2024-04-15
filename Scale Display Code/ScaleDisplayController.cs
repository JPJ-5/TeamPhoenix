/**using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.DataAccessLayer;
using _scaleLog = TeamPhoenix.MusiCali.Services.ScaleDisplay;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("api/ScaleDisplay")]
    public class ScaleDisplayController : ControllerBase
    {
        //private readonly ScaleDisplay scaleDisplay;

        [HttpPost("api/LogScaleDisplay")]
        public IActionResult logScaleUses([FromBody]string username)
        {
            try{
                // Initialize the data access layer
                scaleDisplay = new ScaleDisplay();
                if(scaleDisplay.ScaleDisplayLoggingService(username)){
                    return OK("Logged Scale Usage");
                }
                else{
                    throw new Exception("Log Scale Uses Failed");
                }
            }
            catch(Exception ex){
                Console.WriteLine(ex.toString());
                return BadRequest("Log Scale Uses Failed");
            }
        }
    }
}*/

