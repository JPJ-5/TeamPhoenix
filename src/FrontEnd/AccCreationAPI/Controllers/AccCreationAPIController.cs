//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Net.Http.Headers;
//using System.Runtime.CompilerServices;
//using TeamPhoenix.MusiCali.DataAccessLayer.Models;
//using uC = TeamPhoenix.MusiCali.Services.UserCreation;


//namespace AccCreationAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AccCreationAPIController : ControllerBase
//    {

//        [HttpPost]
//        public JsonResult RegisterUser(string email, DateTime dob, string uname, string fname, string lname, string q, string a )
//        {
//            if(uC.RegisterUser(email, dob, uname, fname, lname, q, a))
//            {
//                return new JsonResult(true);
//            }
//            return new JsonResult(false);

//        }
//    }
//}



using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using uC = TeamPhoenix.MusiCali.Services.UserCreation;


namespace AccCreationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccCreationAPIController : ControllerBase
    {

        [HttpPost("api/NormalAccCreationAPI")]
        public JsonResult RegisterNormalUser(string email, DateTime dob, string uname, string fname, string lname, string q, string a)
        {
            if (uC.RegisterNormalUser(email, dob, uname, fname, lname, q, a))
            {
                return new JsonResult(true);
            }
            return new JsonResult(false);

        }

        [HttpPost("api/AdminlAccCreationAPI")]
        public JsonResult RegisterAdminUser(string email, DateTime dob, string uname, string fname, string lname, string q, string a)
        {
            if (uC.RegisterNormalUser(email, dob, uname, fname, lname, q, a))
            {
                return new JsonResult(true);
            }
            return new JsonResult(false);

        }
    }
}