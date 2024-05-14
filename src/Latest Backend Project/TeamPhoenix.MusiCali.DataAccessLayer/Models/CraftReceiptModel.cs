using Amazon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
   public class CraftReceiptModel
    {
        public int ReceiptID { get; set; }
        public string? CreatorHash { get; set; }
        public string? BuyerHash { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal SellPrice { get; set; }
        public decimal OfferPrice { get; set; }
        public decimal Profit { get; set; }
        public decimal Revenue { get; set; }
        public int Quantity { get; set; }
        public DateTime? SaleDate { get; set; }
        public bool PendingSale { get; set; }
        public CraftReceiptModel() { }
        public CraftReceiptModel (string creatorHash, string buyerHash, string sku, decimal price, decimal offerPrice, decimal profit, decimal revenue, int quantity, bool pendingSale)
        {
            CreatorHash = creatorHash;
            BuyerHash = buyerHash;
            SKU = sku;
            SellPrice = price;
            OfferPrice = offerPrice;
            Profit = profit;
            Revenue = revenue;
            Quantity = quantity;
            SaleDate = DateTime.Now;
            PendingSale = pendingSale;
        }

    }

    public class CraftReceiptWithItemModel
    {
        public int ReceiptID { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal OfferPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Profit { get; set; }
        public decimal Revenue { get; set; }
        public DateTime? SaleDate { get; set; }
        public PaginationItemModel Item { get; set; } = new PaginationItemModel();
    }
}
