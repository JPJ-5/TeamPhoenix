using Amazon.S3.Model.Internal.MarshallTransformations;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class ItemModificationModel
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int StockAvailable { get; set; }
        public decimal ProductionCost { get; set; }
        public bool OfferablePrice { get; set; }
        public string SellerContact { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new List<string>();// Changed to a list of strings for name of the files
        public List<string> VideoUrls { get; set; } = new List<string>(); // Changed to a list of strings for name of the files
        public DateTime Datemodified { get; set; } = DateTime.Now;

        public ItemModificationModel() { }
        public ItemModificationModel(string name, decimal price, string desc,
            int stock, decimal cost, bool offer, string sellerContact, List<string> pic, List<string> video)
        {
            Name = name;
            Price = price;
            Description = desc;
            StockAvailable = stock;
            ProductionCost = cost;
            OfferablePrice = offer;
            SellerContact = sellerContact;
            ImageUrls = pic;
            VideoUrls = video;
            Datemodified = DateTime.Now;

        }
    }
}
