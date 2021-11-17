using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using GSA.Rpc;


namespace App
{
    public interface ICacheService
    {
        Task UpdateGamesSalesCacheAsync(CancellationToken ct);

        Task<ReadGamesWithMoreEUSalesThanNASalesAsyncResult> ReadGamesWithMoreEUSalesThanNASalesAsync();
    }

    public record ReadGamesWithMoreEUSalesThanNASalesAsyncResult
    {
        public bool IsNullOrEmpty { get; init; }

        public IEnumerable<GameSale> GameSales { get; init; } = default!;
    }
}