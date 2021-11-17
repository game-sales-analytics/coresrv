using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.DB.Repository
{
    public interface IGameSalesRepository
    {
        Task SaveBulkGameSalesAsync(IEnumerable<App.Models.GameSale> gameSales);

        Task<IEnumerable<Models.GameSale>> SearchGameByName(string name);

        Task<IEnumerable<Models.GameSale>> GetGameSalesWithMoreSalesInEUThanNA();
    }
}