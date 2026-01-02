using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    /// <summary>
    /// 交易成本的輸入資料模型（由使用者或 API 傳入）。
    /// </summary>
    public class TransactionCostInput
    {
        /// <summary>
        /// 使用者輸入的股票代碼。若填入，Controller 端可能會以資料庫中的收盤價覆寫 <see cref="Price"/>。
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// 單位價格（每股價格），不可為零或負數。
        /// 當同時提供 <see cref="StockCode"/> 時，Controller 可能以資料庫價格覆寫此欄位。
        /// </summary>
        [Required]
        [Range(0.0000001, double.MaxValue)]
        public decimal Price { get; set; }

        /// <summary>
        /// 交易股數，必須為正整數。
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        /// <summary>
        /// 是否為賣出交易；false = 買入、true = 賣出。
        /// </summary>
        public bool IsSell { get; set; }

        /// <summary>
        /// 券商名稱（可選）。表單會將所選券商傳回後端，用來自動填入佣金與最低手續費。
        /// </summary>
        public string Broker { get; set; }

        /// <summary>
        /// 佣金比率（小數），例如 0.001425 代表 0.1425%。
        /// </summary>
        [Range(0, 1)]
        public decimal CommissionRate { get; set; } = 0.001425m;

        /// <summary>
        /// 最低佣金限制（貨幣單位），計算佣金時會與比例計算結果比較，取較大者。
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal MinCommission { get; set; } = 20m;

        /// <summary>
        /// 交易稅率（僅適用於賣出），例如 0.003 代表 0.3%。
        /// </summary>
        [Range(0, 1)]
        public decimal TransactionTaxRate { get; set; } = 0.003m;

        /// <summary>
        /// 其他手續費或固定費用（貨幣單位）。
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal OtherFees { get; set; } = 0m;
    }

    /// <summary>
    /// 計算結果：包含成交金額、各項費用、費用總和與淨收付金額。
    /// </summary>
    public class TransactionCostResult
    {
        /// <summary>
        /// 成交金額（未扣除費用），等於 Price * Quantity。
        /// </summary>
        public decimal GrossAmount { get; set; }

        /// <summary>
        /// 計算出的佣金（已四捨五入至小數第二位）。
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// 交易稅（賣出時才會產生）。
        /// </summary>
        public decimal Tax { get; set; }

        /// <summary>
        /// 其他手續費或額外費用。
        /// </summary>
        public decimal OtherFees { get; set; }

        /// <summary>
        /// 費用總和（Commission + Tax + OtherFees）。
        /// </summary>
        public decimal TotalFees { get; set; }

        /// <summary>
        /// 淨收付金額：
        /// - 賣出時為 正數 = GrossAmount - TotalFees
        /// - 買入時為 負數 = -(GrossAmount + TotalFees)
        /// </summary>
        public decimal NetAmount { get; set; }
    }

    /// <summary>
    /// 提供交易成本計算的靜態方法。
    /// </summary>
    public static class TransactionCostCalculator
    {
        /// <summary>
        /// 計算交易費用與淨收付結果。
        /// </summary>
        /// <param name="input">交易輸入資料，包含價格、張數與各類費率/費用。</param>
        /// <returns>回傳包含成交金額、各項費用、費用總和與淨收付的 <see cref="TransactionCostResult"/>。</returns>
        public static TransactionCostResult Calculate(TransactionCostInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            var gross = input.Price * input.Quantity;
            var commission = Math.Max(gross * input.CommissionRate, input.MinCommission);
            var tax = input.IsSell ? gross * input.TransactionTaxRate : 0m;
            var other = input.OtherFees;
            var totalFees = commission + tax + other;

            // 賣出交易：淨收 = 成交金額 - 總費用
            // 買入交易：淨付 = -(成交金額 + 總費用)
            decimal net = input.IsSell ? gross - totalFees : -(gross + totalFees);

            return new TransactionCostResult
            {
                GrossAmount = decimal.Round(gross, 2),
                Commission = decimal.Round(commission, 2),
                Tax = decimal.Round(tax, 2),
                OtherFees = decimal.Round(other, 2),
                TotalFees = decimal.Round(totalFees, 2),
                NetAmount = decimal.Round(net, 2)
            };
        }
    }
}