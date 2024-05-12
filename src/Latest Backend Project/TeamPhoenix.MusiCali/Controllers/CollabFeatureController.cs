using Microsoft.AspNetCore.Mvc;
//using TeamPhoenix.MusiCali.Logging.Logger;
using TeamPhoenix.MusiCali.DataAccessLayer;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CollabFeatureController : ControllerBase
    {

        [HttpPost("api/SendRequestAPI")]
        public IActionResult SendCollabRequest([FromBody] CollabUsers collab)
        {

            try{   

                Result result = new Result();

                result = CollabFeature.CreateCollabRequest(collab.senderUsername!, collab.receiverUsername!);

                if(result.Success){

                    return Ok(result);
                }

                else{

                    result.Success = false;
                    result.ErrorMessage = "Collab Already Exists";
                    
                    return Ok(result);
                }
            }

            catch(Exception ex){

                throw new Exception("Failed to create collab" + ex.Message);
            }
        }

        [HttpGet("api/LoadViewAPI")]
        public CollabData LoadView (string username){

            try{
            var sentRequests = CollabFeatureDAL.GetSentCollabsByUsername(username);
            var receivedRequests = CollabFeatureDAL.GetReceivedCollabsByUsername(username);
            var acceptedRequests = CollabFeatureDAL.GetAcceptedCollabsByUsername(username);

            CollabData collabs = new CollabData();

            collabs.sentCollabs = sentRequests;
            collabs.receivedCollabs = receivedRequests;
            collabs.acceptedCollabs = acceptedRequests;

            return collabs;
            }

            catch(Exception ex){

                throw new Exception("Failed to load collab view: " + ex.Message);

            }
        }

        [HttpPost("api/AcceptRequestAPI")]
        public IActionResult Accept(CollabUsers collab)
        {

            try
            {
                Result result = CollabFeature.AcceptCollab(collab.receiverUsername!, collab.senderUsername!);

                if(result.Success){

                    return Ok(result);
                }

                else{

                    result.Success = false;
                    result.ErrorMessage = "Failed to accept request";
                    
                    return Ok(result);
                }
            }

            catch (Exception ex)
            {

                throw new Exception("Failed to accept collab; " + ex.Message);

            }
        }

        [HttpGet("api/LoadCollabsAPI")]
        public IActionResult LoadCollabsInView([FromHeader] string userName){

            try
            {
                Console.WriteLine(userName);
                CollabData result = CollabFeature.LoadCollabFeature(userName);

                return Ok(result);
            }

            catch(Exception ex){

                Console.WriteLine("Could not load the collabs" + ex.Message);
                return BadRequest("Collab Not Available Right Now");
            }
        }

        [HttpGet("api/DisplayAvailableUsersAPI")]
        public IActionResult ShowAvailUsers([FromHeader] string userName){

            try{

                List<string> availUsers = CollabFeatureDAL.SearchUsers(userName);

                return Ok(availUsers);
            }

            catch(Exception ex){

                throw new Exception("Could not load available users" + ex.Message);
            }
        }
    }
}