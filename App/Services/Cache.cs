using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using StackExchange.Redis;
using Google.Protobuf.WellKnownTypes;
using App.DB.Repository;
using GSA.Rpc;

namespace App
{
    public class CacheService : ICacheService
    {
        private readonly IGameSalesRepository gameSalesRepository;

        private readonly IDatabase cache;

        public CacheService(IGameSalesRepository _gameSalesRepository, IDatabase _cache)
        {
            gameSalesRepository = _gameSalesRepository;
            cache = _cache;
        }

        public async Task<ReadGamesWithMoreEUSalesThanNASalesAsyncResult> ReadGamesWithMoreEUSalesThanNASalesAsync()
        {
            var cachedResponse = await cache.StringGetAsync("games_with_more_sales_in_eu_than_na");
            if (!cachedResponse.IsNullOrEmpty)
            {
                return new ReadGamesWithMoreEUSalesThanNASalesAsyncResult
                {
                    IsNullOrEmpty = false,
                    GameSales = JsonSerializer.Deserialize<IEnumerable<GameSale>>(cachedResponse, new JsonSerializerOptions
                    {
                        AllowTrailingCommas = false,
                    })!,
                };
            }

            return new ReadGamesWithMoreEUSalesThanNASalesAsyncResult
            {
                IsNullOrEmpty = true,
                GameSales = new List<GameSale>(),
            };
        }

        public async Task UpdateGamesSalesCacheAsync(CancellationToken ct)
        {
            await UpdateGamesWithMoreSalesInEuThanNaCacheAsync(ct);
        }

        private async Task UpdateGamesWithMoreSalesInEuThanNaCacheAsync(CancellationToken ct)
        {
            var games = (await gameSalesRepository.GetGameSalesWithMoreSalesInEUThanNA(ct)).Select(item => new GameSale
            {
                EuSales = item.EuSales,
                Genre = item.Genre,
                GlobalSales = item.GlobalSales,
                Id = item.Id,
                JpSales = item.JpSales,
                Name = item.Name,
                NaSales = item.NaSales,
                OtherSales = item.OtherSales,
                Platform = item.Platform,
                Publisher = item.Publisher,
                Rank = item.Rank,
                RegisteredAt = Timestamp.FromDateTime(item.RegisteredAt),
                Year = item.Year,
            });
            var serializedResult = JsonSerializer.Serialize(games);
            await cache.StringSetAsync("games_with_more_sales_in_eu_than_na", serializedResult);
        }
    }
}