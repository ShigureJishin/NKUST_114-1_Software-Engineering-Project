using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public StockApiController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 取得當日市場漲跌平家數
        /// </summary>
        /// <param name="date">交易日期 (yyyy-MM-dd) 或 "latest"</param>
        /// <returns>JSON: 上漲、下跌、平盤家數、訊息，若 date=latest 則回傳最近有資料日期</returns>
        [HttpGet("market-summary")]
        public async Task<IActionResult> GetMarketSummary(string date)
        {
            DateTime targetDate;

            // 如果傳入 latest，找到最近有資料的日期
            if (string.Equals(date, "latest", StringComparison.OrdinalIgnoreCase))
            {
                var latestStock = await _db.Stocks
                    .Where(s => s.StockDate.HasValue)
                    .OrderByDescending(s => s.StockDate)
                    .FirstOrDefaultAsync();

                if (latestStock == null)
                {
                    return Ok(new
                    {
                        Up = 0,
                        Down = 0,
                        Flat = 0,
                        Message = "資料庫無任何資料",
                        LatestDate = (string?)null
                    });
                }

                targetDate = latestStock.StockDate!.Value.Date;

                // 回傳最近有資料的日期，不做統計時也可直接統計當日
                date = targetDate.ToString("yyyy-MM-dd");
            }
            else
            {
                // 嘗試解析傳入的日期
                if (!DateTime.TryParse(date, out targetDate))
                {
                    return BadRequest(new { Message = "日期格式錯誤" });
                }
            }

            // 統計當日上漲/下跌/平盤
            var data = await _db.Stocks
                .Where(s => s.StockDate.HasValue && s.StockDate.Value.Date == targetDate)
                .GroupBy(s => 1)
                .Select(g => new
                {
                    Up = g.Count(x => x.Change > 0),
                    Down = g.Count(x => x.Change < 0),
                    Flat = g.Count(x => x.Change == 0 || x.Change == null)
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return Ok(new
                {
                    Up = 0,
                    Down = 0,
                    Flat = 0,
                    Message = "當日資料未更新",
                    LatestDate = targetDate.ToString("yyyy-MM-dd")
                });
            }

            return Ok(new
            {
                data.Up,
                data.Down,
                data.Flat,
                Message = "",
                LatestDate = targetDate.ToString("yyyy-MM-dd")
            });
        }
    }
}
