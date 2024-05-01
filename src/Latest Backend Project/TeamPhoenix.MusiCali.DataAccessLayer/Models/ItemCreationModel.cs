using Amazon.S3.Model.Internal.MarshallTransformations;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class ItemCreationModel
    {
        public string Name { get; set; } = string.Empty;
        public string? CreatorHash { get; set; }
        public string? Sku { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int StockAvailable { get; set; }
        public decimal ProductionCost { get; set; }
        public bool OfferablePrice { get; set; }
        public string SellerContact { get; set; } = string.Empty;
        public List<string>? ImageUrls { get; set; } // Changed to a list of strings for name of the files
        public List<string>? VideoUrls { get; set; } // Changed to a list of strings for name of the files
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public bool Listed { get; set; }

        public ItemCreationModel() { }
        public ItemCreationModel(string name, string creatorHash, string sku, decimal price, string desc,
            int stock, decimal cost, bool offer, string sellerContact, List<string> pic, List<string> video, bool listed)
        {
            Name = name;
            CreatorHash = creatorHash;
            Sku = sku;
            Price = price;
            Description = desc;
            StockAvailable = stock;
            ProductionCost = cost;
            OfferablePrice = offer;
            SellerContact = sellerContact;
            ImageUrls = pic;
            VideoUrls = video;
            DateCreated = DateTime.Now;
            Listed = listed;
        }
    }
}
