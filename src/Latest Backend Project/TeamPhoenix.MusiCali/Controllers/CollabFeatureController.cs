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
        public Result SendCollabRequest([FromBody] CollabUsers collab)
        {

            try{   

                Result result = new Result();

                result = CollabFeature.CreateCollabRequest(collab.senderUsername, collab.receiverUsername);

                if(result.Success){

                    return result;
                }

                else{

                    result.Success = false;
                    result.ErrorMessage = "Failed to create request";
                    
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
        public Result Accept(CollabUsers collab)
        {

            try
            {
                Result result = CollabFeature.AcceptCollab(collab.receiverUsername, collab.senderUsername);

                return result;
            }

            catch (Exception ex)
            {

                throw new Exception("Failed accept collab; " + ex.Message);

            }
        }
    }
}