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
    }
}