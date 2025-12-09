using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StocksController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 200, string sort = "Id", string sortDir = "asc", string searchField = null, string searchTerm = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 200;
            sort = string.IsNullOrEmpty(sort) ? "Id" : sort;
            sortDir = string.IsNullOrEmpty(sortDir) ? "asc" : sortDir.ToLower();

            var query = _db.Stocks.AsQueryable();

            // Apply filtering by chosen field
            if (!string.IsNullOrEmpty(searchField) && !string.IsNullOrEmpty(searchTerm))
            {
                if (searchField == "StockCode")
                {
                    query = query.Where(s => s.StockCode.Contains(searchTerm));
                }
                else if (searchField == "StockName")
                {
                    query = query.Where(s => s.StockName.Contains(searchTerm));
                }
            }

            // Apply sorting using actual property names
            switch (sort)
            {
                case "StockCode":
                    query = sortDir == "asc" ? query.OrderBy(s => s.StockCode) : query.OrderByDescending(s => s.StockCode);
                    break;
                case "StockName":
                    query = sortDir == "asc" ? query.OrderBy(s => s.StockName) : query.OrderByDescending(s => s.StockName);
                    break;
                case "ClosingPrice":
                    query = sortDir == "asc" ? query.OrderBy(s => s.ClosingPrice) : query.OrderByDescending(s => s.ClosingPrice);
                    break;
                case "CreatedAt":
                    query = sortDir == "asc" ? query.OrderBy(s => s.CreatedAt) : query.OrderByDescending(s => s.CreatedAt);
                    break;
                default:
                    query = sortDir == "asc" ? query.OrderBy(s => s.Id) : query.OrderByDescending(s => s.Id);
                    break;
            }

            var total = await query.CountAsync();
            var stocks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new StockListViewModel
            {
                Stocks = stocks,
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                Sort = sort,
                SortDir = sortDir,
                SearchField = searchField,
                SearchTerm = searchTerm
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var stock = await _db.Stocks.FirstOrDefaultAsync(s => s.Id == id.Value);
            if (stock == null) return NotFound();

            return View(stock);
        }
    }
}
