using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class ItemCreationModel
    {
        public string Name { get; set; }
        public string CreatorHash { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int StockAvailable { get; set; }
        public decimal ProductionCost { get; set; }
        public bool OfferablePrice { get; set; }
        public string SellerContact { get; set; }
        public List<string> ImageUrls { get; set; } // Changed to a list of strings for URLs
        public List<string> VideoUrls { get; set; } // Changed to a list of strings for URLs
        public DateTime DateCreated { get; set; } = DateTime.Now;


    }
}
