//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using rU = TeamPhoenix.MusiCali.Services.RecoverUser;
//using TeamPhoenix.MusiCali.DataAccessLayer.Models;
//using Mysqlx;
//using TeamPhoenix.MusiCali.Models;

//namespace TeamPhoenix.MusiCali.Controllers
//{
//    [Route("[controller]")]
//    [ApiController]
//    public class RecoverUserController : ControllerBase
//    {

//        [HttpPost("submit")]
//        public JsonResult RecoverUser([FromBody] RecoverRequest userData)
//        {

//            Dictionary<Boolean, string> result = new Dictionary<Boolean, string>();
//            if (rU.recoverDisabledAccount(userData.username, userData.otp))
//            {
//                result.Add(true, "Recovered User Successfully");
//                return new JsonResult(Ok(result));
//            }
//            result.Add(false, "Unable To Recover User");
//            return new JsonResult(NotFound(result));
//        }

//        [HttpPost("/api/DisableUser")]
//        public JsonResult disableUser(string userName)
//        {
//            Dictionary<Boolean, string> result = new Dictionary<Boolean, string>();
//            if (rU.DisableUser(userName))
//            {
//                result.Add(true, "Disabled User Successfully");
//                return new JsonResult(Ok(result));
//            }
//            result.Add(false, "Unable To Disable User");
//            return new JsonResult(NotFound(result));
//        }

//        [HttpPost("/api/EnableUSer")]
//        public JsonResult enableUser(string userName)
//        {
//            Dictionary<Boolean, string> result = new Dictionary<Boolean, string>();
//            if (rU.EnableUser(userName))
//            {
//                result.Add(true, "Enabled User Successfully");
//                return new JsonResult(Ok(result));
//            }
//            result.Add(false, "Unable To Enable User");
//            return new JsonResult(NotFound(result));
//        }
//    }
//}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rU = TeamPhoenix.MusiCali.Services.RecoverUser;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Mysqlx;
using Microsoft.IdentityModel.Tokens;
using MySqlX.XDevAPI.Common;



namespace TeamPhoenix.MusiCali.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RecoverUserController : ControllerBase
    {

        //[HttpGet("/api/GetEmails")]
        //public JsonResult getEmailsService()
        //{
        //    try
        //    {
        //        List<string> result = rU.getEmails();
        //        if (!result.IsNullOrEmpty())
        //        {
        //            return new JsonResult(Ok(result));
        //        }
        //        throw new Exception("NullResult");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine($"Error: {e}");
        //        return new JsonResult(NotFound(null));
        //    }

        //}

        [HttpPost("/api/RecoverUser")]
        public JsonResult RecoverUser([FromHeader]string userName)
        {
            Dictionary<Boolean, string> result = new Dictionary<Boolean, string>();
            if (rU.SendRecoveryEmail(userName))
            {
                result.Add(true, "OTP sucessfully sent to recovery email, Secondary email has now become primary source for otp.");
                return new JsonResult(Ok(result));
            }
            result.Add(false, "Unable To Recover User");
            return new JsonResult(NotFound(result));
        }

        [HttpPost("/api/DisableUser")]
        public JsonResult disableUser(string userName)
        {
            Dictionary<Boolean, string> result = new Dictionary<Boolean, string>();
            if (rU.DisableUser(userName))
            {
                result.Add(true, "Disabled User Successfully");
                return new JsonResult(Ok(result));
            }
            result.Add(false, "Unable To Disable User");
            return new JsonResult(NotFound(result));
        }

        [HttpPost("/api/EnableUSer")]
        public JsonResult enableUser(string userName)
        {
            Dictionary<Boolean, string> result = new Dictionary<Boolean, string>();
            if (rU.EnableUser(userName))
            {
                result.Add(true, "Enabled User Successfully");
                return new JsonResult(Ok(result));
            }
            result.Add(false, "Unable To Enable User");
            return new JsonResult(NotFound(result));
        }
    }
}