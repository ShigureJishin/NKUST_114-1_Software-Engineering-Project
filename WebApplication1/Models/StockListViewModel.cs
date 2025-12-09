using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class StockListViewModel
    {
        public IEnumerable<Stock> Stocks { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
