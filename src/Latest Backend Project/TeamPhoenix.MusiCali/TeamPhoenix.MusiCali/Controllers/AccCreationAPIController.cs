﻿using Microsoft.AspNetCore.Http;
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
        public JsonResult RegisterNormalUser(string email, DateTime dob, string uname, string bmail)
        {
            if (uC.RegisterNormalUser(email, dob, uname, bmail))
            {
                return new JsonResult(true);
            }
            return new JsonResult(false);

        }

        [HttpPost("api/AdminlAccCreationAPI")]
        public JsonResult RegisterAdminUser(string email, DateTime dob, string uname, string bmail)
        {
            if (uC.RegisterNormalUser(email, dob, uname, bmail))
            {
                return new JsonResult(true);
            }
            return new JsonResult(false);

        }
    }
}
