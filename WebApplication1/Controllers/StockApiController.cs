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
        /// <param name="date">交易日期 (yyyy-MM-dd)。未提供則自動使用最近有資料日期</param>
        /// <returns>JSON: 上漲、下跌、平盤家數、訊息，並回傳最近有資料日期</returns>
        [HttpGet("market-summary")]
        public async Task<IActionResult> GetMarketSummary([FromQuery] DateOnly? date = null)
        {
            DateTime targetDate;

            // 若未提供 date，找到最近有資料的日期 (latest)
            if (date == null)
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
            }
            else
            {
                targetDate = date.Value.ToDateTime(TimeOnly.MinValue);
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

        /// <summary>
        /// 取得最近有資料日期的市場漲跌平家數
        /// </summary>
        [HttpGet("market-summary/latest")]
        public Task<IActionResult> GetMarketSummaryLatest()
            => GetMarketSummary(date: null);

        /// <summary>
        /// 取得指定日期的所有股票資訊
        /// </summary>
        /// <param name="date">交易日期 (yyyy-MM-dd)。未提供則自動使用最近有資料日期</param>
        /// <returns>JSON: 日期與該日所有股票資料</returns>
        [HttpGet("stocks")]
        public async Task<IActionResult> GetStocksByDate([FromQuery] DateOnly? date = null)
        {
            DateTime targetDate;

            if (date == null)
            {
                var latestStock = await _db.Stocks
                    .AsNoTracking()
                    .Where(s => s.StockDate.HasValue)
                    .OrderByDescending(s => s.StockDate)
                    .Select(s => s.StockDate)
                    .FirstOrDefaultAsync();

                if (latestStock == null)
                {
                    return Ok(new
                    {
                        Date = (string?)null,
                        Count = 0,
                        Items = Array.Empty<object>(),
                        Message = "資料庫無任何資料"
                    });
                }

                targetDate = latestStock.Value.Date;
            }
            else
            {
                targetDate = date.Value.ToDateTime(TimeOnly.MinValue);
            }

            var items = await _db.Stocks
                .AsNoTracking()
                .Where(s => s.StockDate.HasValue && s.StockDate.Value.Date == targetDate)
                .OrderBy(s => s.StockCode)
                .Select(s => new
                {
                    s.Id,
                    s.StockCode,
                    s.StockName,
                    StockDate = s.StockDate,
                    s.TradeVolume,
                    s.TradeValue,
                    s.OpeningPrice,
                    s.HighestPrice,
                    s.LowestPrice,
                    s.ClosingPrice,
                    s.Change,
                    s.Transaction,
                    s.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                Date = targetDate.ToString("yyyy-MM-dd"),
                Count = items.Count,
                Items = items,
                Message = items.Count == 0 ? "當日資料未更新" : ""
            });
        }

        /// <summary>
        /// 取得最近有資料日期的所有股票資訊
        /// </summary>
        [HttpGet("stocks/latest")]
        public Task<IActionResult> GetStocksLatest()
            => GetStocksByDate(date: null);

        /// <summary>
        /// 依代碼或名稱搜尋單筆最近資料（用於前端快速查價）
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string field, [FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return BadRequest(new { Message = "term is required" });

            var q = term.Trim();

            if (string.Equals(field, "StockCode", StringComparison.OrdinalIgnoreCase))
            {
                var code = q.ToUpper();
                var item = await _db.Stocks
                    .AsNoTracking()
                    .Where(s => s.StockCode != null && s.StockCode.Trim().ToUpper() == code)
                    .OrderByDescending(s => s.StockDate)
                    .FirstOrDefaultAsync();

                if (item == null) return NotFound(new { Message = "NotFound" });

                return Ok(new
                {
                    item.Id,
                    item.StockCode,
                    item.StockName,
                    item.StockDate,
                    item.ClosingPrice,
                    item.TradeVolume,
                    item.TradeValue
                });
            }

            // default: search by name (contains, case-insensitive)
            var up = q.ToUpper();
            var found = await _db.Stocks
                .AsNoTracking()
                .Where(s => s.StockName != null && s.StockName.ToUpper().Contains(up))
                .OrderByDescending(s => s.StockDate)
                .FirstOrDefaultAsync();

            if (found == null) return NotFound(new { Message = "NotFound" });

            return Ok(new
            {
                found.Id,
                found.StockCode,
                found.StockName,
                found.StockDate,
                found.ClosingPrice,
                found.TradeVolume,
                found.TradeValue
            });
        }

        /// <summary>
        /// 以股票代碼取得該股票最近一筆資料（若有多日資料則取最近日期）
        /// </summary>
        [HttpGet("stock/{code}")]
        public async Task<IActionResult> GetStockByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return BadRequest(new { Message = "股票代碼為空" });

            var trimmed = code.Trim().ToUpper();

            var stock = await _db.Stocks
                .AsNoTracking()
                .Where(s => s.StockCode != null && s.StockCode.Trim().ToUpper() == trimmed)
                .OrderByDescending(s => s.StockDate ?? DateTime.MinValue)
                .FirstOrDefaultAsync();

            if (stock == null)
            {
                return NotFound(new { Message = "找不到此股票代碼" });
            }

            return Ok(new
            {
                stock.Id,
                stock.StockCode,
                stock.StockName,
                StockDate = stock.StockDate,
                stock.TradeVolume,
                stock.TradeValue,
                stock.OpeningPrice,
                stock.HighestPrice,
                stock.LowestPrice,
                stock.ClosingPrice,
                stock.Change,
                stock.Transaction,
                stock.CreatedAt
            });
        }
    }
}
