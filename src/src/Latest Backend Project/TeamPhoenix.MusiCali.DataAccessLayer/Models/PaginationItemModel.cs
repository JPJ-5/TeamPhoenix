using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class PaginationItemModel
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Sku { get; set; }
        public int StockAvailable { get; set; }
        public string? FirstImage { get; set; }

        public PaginationItemModel() { }
        
    }



    //public class PaginationItemWithReceiptsModel
    //{
    //    public PaginationItemModel? Item { get; set; }
    //    public List<CraftReceiptModel> PendingReceipts { get; set; } = new List<CraftReceiptModel>();
    //}
}

