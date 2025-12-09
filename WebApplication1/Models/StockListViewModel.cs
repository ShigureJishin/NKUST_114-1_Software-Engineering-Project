using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class StockListViewModel
    {
        public IEnumerable<Stock> Stocks { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string Sort { get; set; }
        public string SortDir { get; set; }
<<<<<<< HEAD

        // Search inputs
        public string SearchCode { get; set; }
        public string SearchName { get; set; }

        // Search modes (contains, startswith, exact)
        public string SearchCodeMode { get; set; }
        public string SearchNameMode { get; set; }

        // Optional lists (not required by UI but populated by controller if needed)
        public IEnumerable<string> SearchCodeOptions { get; set; }
        public IEnumerable<string> SearchNameOptions { get; set; }

        // New: single field selection and term input
        public string SearchField { get; set; } // "StockCode" or "StockName"
        public string SearchTerm { get; set; }
=======
>>>>>>> a7e0f5ca530cf2f64d9adfbd1523ed63bb5a3718
    }
}
