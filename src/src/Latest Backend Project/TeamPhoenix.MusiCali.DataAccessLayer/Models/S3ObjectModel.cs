using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class S3ObjectModel
    {
        public string? Name { get; set; }
        public string? PresignedUrl { get; set; }
    }
}
