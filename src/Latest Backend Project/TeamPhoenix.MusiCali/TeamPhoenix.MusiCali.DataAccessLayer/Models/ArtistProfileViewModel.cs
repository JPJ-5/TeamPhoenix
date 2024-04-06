using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class ArtistProfileViewModel
    {
        public List<string> ProfileInfo { get; set; }
        public List<List<string>> LocalFileInfo { get; set; }
    }
}