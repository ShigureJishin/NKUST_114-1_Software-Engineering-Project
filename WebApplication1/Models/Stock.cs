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
    }
}