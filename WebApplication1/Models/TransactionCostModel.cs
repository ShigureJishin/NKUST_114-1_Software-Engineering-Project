using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class TransactionCostInput
    {
        [Required]
        [Range(0.0000001, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // 買入 false / 賣出 true
        public bool IsSell { get; set; }

        [Range(0, 1)]
        public decimal CommissionRate { get; set; } = 0.001425m; // 預設手續費率 (可調整)

        [Range(0, double.MaxValue)]
        public decimal MinCommission { get; set; } = 20m; // 預設最低手續費

        [Range(0, 1)]
        public decimal TransactionTaxRate { get; set; } = 0.003m; // 預設交易稅（賣出時適用）

        [Range(0, double.MaxValue)]
        public decimal OtherFees { get; set; } = 0m;
    }

    public class TransactionCostResult
    {
        public decimal GrossAmount { get; set; }    // price * qty
        public decimal Commission { get; set; }
        public decimal Tax { get; set; }
        public decimal OtherFees { get; set; }
        public decimal TotalFees { get; set; }      // Commission + Tax + OtherFees
        public decimal NetAmount { get; set; }      // 賣出：Gross - TotalFees ; 買入：- (Gross + TotalFees)
    }

    public static class TransactionCostCalculator
    {
        public static TransactionCostResult Calculate(TransactionCostInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            var gross = input.Price * input.Quantity;
            var commission = Math.Max(gross * input.CommissionRate, input.MinCommission);
            var tax = input.IsSell ? gross * input.TransactionTaxRate : 0m;
            var other = input.OtherFees;
            var totalFees = commission + tax + other;

            decimal net;
            if (input.IsSell)
            {
                net = gross - totalFees; // 賣出淨收
            }
            else
            {
                net = -(gross + totalFees); // 買入淨支出表示為負數
            }

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