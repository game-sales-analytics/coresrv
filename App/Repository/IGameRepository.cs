using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Threading;


namespace App.DB.Repository
{
    public interface IGameSalesRepository
    {
        Task SaveBulkGameSalesAsync(IEnumerable<App.Models.GameSale> gameSales, CancellationToken ct);

        Task<IEnumerable<Models.GameSale>> SearchGameByName(string name, CancellationToken ct);

        Task<IEnumerable<Models.GameSale>> GetGameSalesWithMoreSalesInEUThanNA(CancellationToken ct);

        Task<Models.GameSale?> GetGameSalesByRank(ulong rank, CancellationToken ct);

        Task<List<KeyValuePair<string, List<Models.GameSale>>>> GetTopNGamesOfPlatforms(ulong n, CancellationToken ct);

        Task<List<KeyValuePair<string, float>>> GetTotalGameSalesInYearsRangeByGenre(uint startYear, uint endYear, CancellationToken ct);

        Task<List<KeyValuePair<uint, float>>> GetYearlyTotalGameSalesInRange(uint startYear, uint endYear, CancellationToken ct);

        Task<IList<Models.GameSale>> GetGameSalesInIds(IEnumerable<string> ids, CancellationToken ct);

        Task<ImmutableDictionary<string, ImmutableList<TotalPublisherGameSalesInYear>>> GetTotalPublishersGameSalesInYearsRange(IEnumerable<string> publishers, uint startYear, uint endYear, CancellationToken ct);
    }

    public record TotalPublisherGameSalesInYear
    {
        public float TotalSales { get; init; } = default!;

        public uint Year { get; init; } = default!;
    };
}