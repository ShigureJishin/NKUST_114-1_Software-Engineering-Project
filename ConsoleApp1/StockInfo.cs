using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ConsoleApp1
{
    internal class StockInfo
    {
        [JsonPropertyName("Date")]
        public string StockDate { get; set; }

        [JsonPropertyName("Code")]
        public string StockCode { get; set; }

        [JsonPropertyName("Name")]
        public string StockName { get; set; }

        [JsonPropertyName("TradeVolume")]
        public string TradeVolume { get; set; }

        [JsonPropertyName("TradeValue")]
        public string TradeValue { get; set; }

        [JsonPropertyName("OpeningPrice")]
        public string OpeningPrice { get; set; }

        [JsonPropertyName("HighestPrice")]
        public string HighestPrice { get; set; }

        [JsonPropertyName("LowestPrice")]
        public string LowestPrice { get; set; }

        [JsonPropertyName("ClosingPrice")]
        public string ClosingPrice { get; set; }

        [JsonPropertyName("Change")]
        public string Change { get; set; }

        [JsonPropertyName("Transaction")]
        public string Transaction { get; set; }
    }
}

