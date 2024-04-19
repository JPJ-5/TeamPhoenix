﻿using Microsoft.AspNetCore.Mvc;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using BB = TeamPhoenix.MusiCali.Services.BingoBoard;

namespace TeamPhoenix.MusiCali.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BingoBoardController : Controller
    {
        [HttpPost("api/BingoBoardLoadGigs")]
        public ActionResult ViewMultipleGigs([FromBody] BingoBoardRequest BBReq)
        {
            GigSet? gigSummaries = BB.ViewMultGigSummary(BBReq.NumberOfGigs, BBReq.Username, BBReq.Offset);
            if (gigSummaries == null)
            {
                return NotFound("Error retrieving gigs");
            }
            return Ok(gigSummaries);
        }

        [HttpGet("api/BingoBoardRetrieveGigTableSize")]
        public ActionResult RetrieveGigTableSize()
        {
            int gigTableSize = BB.ReturnGigNum();
            if(gigTableSize <= 0) { return NotFound("Error retrieving Gig Table size"); }
            return Ok(gigTableSize);
        }
    }
}