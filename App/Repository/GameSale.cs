using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using App.DB.Models;

namespace App.DB.Repository
{
    public class GameSalesRepository : IGameSalesRepository
    {
        private readonly GameSalesContext _context;

        private readonly ILogger<GameSalesRepository> _logger;

        public GameSalesRepository(
            GameSalesContext context,
            ILogger<GameSalesRepository> logger
        )
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveBulkGameSalesAsync(IEnumerable<App.Models.GameSale> gameSales, CancellationToken ct)
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
            await _context.GameSales.AddRangeAsync(temp, ct);

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex)
            {
                throw ex.InnerException switch
                {
                    PostgresException exception when exception.Severity == "ERROR" && exception.SqlState == PostgresErrorCodes.UniqueViolation => new DuplicateRecordException(),
                    _ => ex,
                };
            }
        }

        public async Task<IEnumerable<Models.GameSale>> SearchGameByName(string name, CancellationToken ct)
        {
            return await _context.GameSales
                .AsNoTracking()
                .Where(g => EF.Functions.ToTsVector("english", g.Name).Matches(name))
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Models.GameSale>> GetGameSalesWithMoreSalesInEUThanNA(CancellationToken ct)
        {
            return await _context.GameSales.AsNoTracking().Where(g => g.EuSales > g.NaSales).ToListAsync(ct);
        }

        public async Task<GameSale?> GetGameSalesByRank(ulong rank, CancellationToken ct)
        {
            try
            {
                return await _context.GameSales.AsNoTracking().SingleOrDefaultAsync(g => g.Rank.Equals(rank), ct);
            }
            catch (System.ArgumentNullException)
            {
                return await Task.FromResult<GameSale?>(null);
            }
            catch (System.InvalidOperationException ex)
            {
                _logger.LogError(ex, "multiple games with the same rank found");
                return null;
            }
        }

        public async Task<List<KeyValuePair<string, List<GameSale>>>> GetTopNGamesOfPlatforms(ulong n, CancellationToken ct)
        {
            return await _context.GameSales.AsNoTracking().Select(g => g.Platform).Distinct().Select(platform => KeyValuePair.Create(platform, _context.GameSales.AsNoTracking().Where(g => g.Platform.Equals(platform)).OrderBy(g => g.Rank).Take((int)n).ToList())).ToListAsync(ct);
        }

        public async Task<List<KeyValuePair<string, float>>> GetTotalGameSalesInYearsRangeByGenre(uint startYear, uint endYear, CancellationToken ct)
        {
            var query = from g in _context.GameSales
                        where g.Year <= endYear && g.Year >= startYear
                        group g.GlobalSales by g.Genre into gg
                        select KeyValuePair.Create(gg.Key, gg.Sum());

            return await query.ToListAsync();
        }

        public async Task<List<KeyValuePair<uint, float>>> GetYearlyTotalGameSalesInRange(uint startYear, uint endYear, CancellationToken ct)
        {
            var query = from g in _context.GameSales
                        where g.Year <= endYear && g.Year >= startYear
                        group g.GlobalSales by g.Year into gg
                        orderby gg.Key ascending
                        select KeyValuePair.Create(gg.Key, gg.Sum());

            return await query.ToListAsync();
        }

        public async Task<IList<GameSale>> GetGameSalesInIds(IEnumerable<string> ids, CancellationToken ct)
        {
            var query = from g in _context.GameSales where ids.Contains(g.Id) select g;

            return await query.ToListAsync();
        }

        public async Task<ImmutableDictionary<string, ImmutableList<TotalPublisherGameSalesInYear>>> GetTotalPublishersGameSalesInYearsRange(IEnumerable<string> publishers, uint startYear, uint endYear, CancellationToken ct)
        {
            var query = from g in _context.GameSales
                        where g.Year >= startYear && g.Year <= endYear && publishers.Contains(g.Publisher)
                        orderby g.Year
                        group g.GlobalSales by new { Year = g.Year, Publisher = g.Publisher } into gg
                        select KeyValuePair.Create(new { Year = gg.Key.Year, Publisher = gg.Key.Publisher }, gg.Sum());

            var result = await query.ToListAsync(ct);

            var dict = new Dictionary<string, ImmutableList<TotalPublisherGameSalesInYear>>(publishers.Count());

            foreach (var item in result)
            {
                var newRecordToBeAdded = new TotalPublisherGameSalesInYear
                {
                    TotalSales = item.Value,
                    Year = item.Key.Year,
                };
                dict[item.Key.Publisher] = dict.ContainsKey(item.Key.Publisher) ? dict[item.Key.Publisher].Add(newRecordToBeAdded) : ImmutableList.Create(newRecordToBeAdded);
            }

            foreach (var k in dict.Keys)
            {
                dict[k] = dict[k].OrderBy(x => x.Year).ToImmutableList();
            }

            return dict.ToImmutableDictionary();
        }

        public async Task<ImmutableList<GameSale>> Get5MostSoldGamesByYearAndPlatform(uint year, string platform, CancellationToken ct)
        {
            var result = await _context.GameSales
                .Where(g => g.Year.Equals(year) && g.Platform.Equals(platform))
                .Take(5)
                .OrderBy(g => g.Rank)
                .Select(g => g)
                .ToListAsync(ct);

            return result.ToImmutableList();
        }
    }
}