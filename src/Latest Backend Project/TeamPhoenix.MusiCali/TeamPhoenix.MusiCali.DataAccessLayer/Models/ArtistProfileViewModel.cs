using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using Microsoft.AspNetCore.Http; // Import the namespace for IFormFile

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class ArtistProfileViewModel
    {
        public string Occupation { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public string File0 { get; set; } // Change type to IFormFile
        public string File0Ext { get; set; }
        public string File1 { get; set; } // Change type to IFormFile
        public string File1Ext { get; set; } // Change type to IFormFile
        public string File1Genre { get; set; }
        public string File1Desc { get; set; }
        public string File2 { get; set; } // Change type to IFormFile
        public string File2Ext { get; set; } // Change type to IFormFile
        public string File2Genre { get; set; }
        public string File2Desc { get; set; }
        public string File3 { get; set; } // Change type to IFormFile
        public string File3Ext { get; set; } // Change type to IFormFile
        public string File3Genre { get; set; }
        public string File3Desc { get; set; }
        public string File4 { get; set; } // Change type to IFormFile
        public string File4Ext { get; set; } // Change type to IFormFile
        public string File4Genre { get; set; }
        public string File4Desc { get; set; }
        public string File5 { get; set; } // Change type to IFormFile
        public string File5Ext { get; set; } // Change type to IFormFile
        public string File5Genre { get; set; }
        public string File5Desc { get; set; }
    }
}
