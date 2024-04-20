using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.Logging.Logger;
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
        public Result SendCollabRequest([FromBody] senderUsername, receiverUsername)
        {

            try{   

                Result new = new Result();

                result = CollabFeature.CreateCollabRequest(senderUsername, receiverUsername);

                if(result == true){

                    return result;
                }
            }

            catch(Exception ex){

                throw new Exception("Failed to create collab" + ex.Message);
            }
        }

        [HttpPost("api/LoadViewAPI")]
        public CollabData LoadView (string username){

            try{
            var sentRequests = CollabFeatureDAL.GetSentCollabsByUsername(username);
            var receivedRequests = CollabFeatureDAL.GetReceivedCollabsByUsername(username);
            var acceptedRequests = CollabFeatureDAL.GetAcceptedCollabsByUsername(username);

            return new CollabData { sentRequests, receivedRequests, acceptedRequests};
            }

            catch(Exception ex){

                throw new Exception("Failed to load collab view: " + ex.Message);

            }
        }
    }
}