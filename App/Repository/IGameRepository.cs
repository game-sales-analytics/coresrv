using System.Collections.Generic;
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
    }
}