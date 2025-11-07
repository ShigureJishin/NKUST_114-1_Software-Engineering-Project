using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1
{
    [Index(nameof(StockCode))]
    [Index(nameof(StockDate))]
    internal class StockInfoEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // 主鍵，自動遞增

        [MaxLength(20)]
        public string StockCode { get; set; }  // 股票代碼

        [MaxLength(100)]
        public string StockName { get; set; }  // 股票名稱

        public DateTime StockDate { get; set; }  // 交易日期

        public long TradeVolume { get; set; }    // 成交股數
        public long TradeValue { get; set; }     // 成交金額
        public decimal OpeningPrice { get; set; } // 開盤價
        public decimal HighestPrice { get; set; } // 最高價
        public decimal LowestPrice { get; set; }  // 最低價
        public decimal ClosingPrice { get; set; } // 收盤價
        public decimal Change { get; set; }       // 漲跌
        public int Transaction { get; set; }      // 成交筆數

        public DateTime CreatedAt { get; set; } = DateTime.Now; // 建立時間
    }
}
