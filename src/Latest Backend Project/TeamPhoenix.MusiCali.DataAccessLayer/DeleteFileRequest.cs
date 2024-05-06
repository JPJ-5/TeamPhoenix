using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class DeleteFileRequest
    {
        public string? Username { get; set; }

        public int? SlotNumber { get; set; }
    }
}