using System;

namespace WebApplication1.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public decimal ClosingPrice { get; set; }
        public DateTime CreatedAt { get; set; }

        // New fields for details page
        public DateTime? StockDate { get; set; }
        public long? TradeVolume { get; set; }
        public long? TradeValue { get; set; }
        public decimal? OpeningPrice { get; set; }
        public decimal? HighestPrice { get; set; }
        public decimal? LowestPrice { get; set; }
        public decimal? Change { get; set; }
        public int? Transaction { get; set; }
    }
}