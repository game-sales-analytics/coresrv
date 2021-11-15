using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using App;

namespace App.DB.Repository
{
    public class GameSalesRepository
    {
        private readonly GameSalesContext _context;

        public GameSalesRepository(
            GameSalesContext context
        )
        {
            _context = context;
        }

        public async Task SaveBulkGameSalesAsync(IEnumerable<App.Models.GameSale> gameSales)
        {
            var temp = gameSales.Select(g => new App.DB.Models.GameSale
            {
                EuSales = g.EuSales,
                Genre = g.Genre,
                GlobalSales = g.GlobalSales,
                OtherSales = g.OtherSales,
                JpSales = g.JpSales,
                Id = g.Id,
                Name = g.Name,
                Rank = g.Rank,
                Publisher = g.Publisher,
                Year = g.Year,
                RegisteredAt = g.RegisteredAt,
                Platform = g.Platform,
                NaSales = g.NaSales,
            });

            await _context.GameSales.AddRangeAsync(temp);
            await _context.SaveChangesAsync();
        }
    }
}