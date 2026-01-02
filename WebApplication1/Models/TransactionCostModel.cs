using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class TransactionCostInput
    {
        // 如果填入股票代碼，Controller 會以資料庫的收盤價覆寫 Price
        public string StockCode { get; set; }

        [Required]
        [Range(0.0000001, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // �R�J false / ��X true
        public bool IsSell { get; set; }

        [Range(0, 1)]
        public decimal CommissionRate { get; set; } = 0.001425m; // �w�]����O�v (�i�վ�)

        [Range(0, double.MaxValue)]
        public decimal MinCommission { get; set; } = 20m; // �w�]�̧C����O

        [Range(0, 1)]
        public decimal TransactionTaxRate { get; set; } = 0.003m; // �w�]����|�]��X�ɾA�Ρ^

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
        public decimal NetAmount { get; set; }      // ��X�GGross - TotalFees ; �R�J�G- (Gross + TotalFees)
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
                net = gross - totalFees; // ��X�b��
            }
            else
            {
                net = -(gross + totalFees); // �R�J�b��X���ܬ��t��
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