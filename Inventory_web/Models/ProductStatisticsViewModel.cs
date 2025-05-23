using Microsoft.AspNetCore.Mvc;

namespace Inventoryweb.Models
{
    public class ProductStatisticsViewModel
    {
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public string TopCategory { get; set; }
    }
}
