
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1
{
	public class StockDbContext : DbContext
	{
	internal DbSet<StockInfoEntity> Stocks { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// LocalDB 預設連線字串
			optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=StockDb;Trusted_Connection=True;MultipleActiveResultSets=true");
		}
	}
}
