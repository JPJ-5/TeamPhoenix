using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class FileUploadViewModel
    {
        public string? Username { get; set; }
        public int Slot { get; set; }
        public IFormFile? File { get; set; }
        public string? Genre { get; set; }
        public string? Desc { get; set; }
    }
}
