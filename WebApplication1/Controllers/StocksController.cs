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

        public async Task<IActionResult> Index(int page = 1, int pageSize = 200)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 200;

            var total = await _db.Stocks.CountAsync();
            var stocks = await _db.Stocks
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new StockListViewModel
            {
                Stocks = stocks,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };

            return View(vm);
        }
    }
}
