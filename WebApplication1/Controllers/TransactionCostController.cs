using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class TransactionCostController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<TransactionCostController> _logger;

        public TransactionCostController(ApplicationDbContext db, ILogger<TransactionCostController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: /TransactionCost/Index
        [HttpGet]
        public IActionResult Index()
        {
            // 建立預設輸入模型
            var model = new TransactionCostInput
            {
                Price = 10m,
                Quantity = 100
            };

            return View(model);
        }

        // POST: /TransactionCost/Index
        [HttpPost]
        public IActionResult Index(TransactionCostInput input)
        {
            // 若有填入股票代碼，優先以資料庫收盤價覆寫輸入價格
            if (!string.IsNullOrWhiteSpace(input.StockCode))
            {
                var code = input.StockCode?.Trim();
                var codeNorm = code?.ToUpper();
                var stock = _db.Stocks.FirstOrDefault(s => s.StockCode != null && s.StockCode.Trim().ToUpper() == codeNorm);
                if (stock == null)
                {
                    ModelState.AddModelError("StockCode", "找不到此股票代碼於資料庫。");
                    return View(input);
                }

                input.Price = stock.ClosingPrice;

                // 移除舊的 Price model state 以便重新驗證新的價格
                ModelState.Remove(nameof(input.Price));
                // 重新驗證模型（包含更新後的 Price）
                TryValidateModel(input);
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            _logger.LogInformation("計算交易成本輸入: {@input}", input);

            // 執行計算
            var result = TransactionCostCalculator.Calculate(input);

            // 將結果傳入 View
            ViewBag.Result = result;

            return View(input);
        }
    }
}
