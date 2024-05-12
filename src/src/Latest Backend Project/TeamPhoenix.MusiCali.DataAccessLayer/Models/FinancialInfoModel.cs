using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamPhoenix.MusiCali.DataAccessLayer.Models
{
    public class FinancialInfoModel
    {
        public int? financialMonth { get; set; }
        public int? financialQuater { get; set; }
        public int financialYear { get; set; }
        public decimal financialProfit { get; set; }
        public decimal financialRevenue { get; set; }
        public int sales { get; set; }

    }
}
