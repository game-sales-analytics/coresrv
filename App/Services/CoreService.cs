using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GSA.Services;
using App.DB.Repository;


namespace App
{
    public class MainService : CoreService.CoreServiceBase
    {
        private readonly ILogger<MainService> _logger;

        private readonly IDatabase _cache;

        private readonly IConfiguration _configuration;

        private readonly IGameSalesRepository _repo;

        public MainService(
            ILogger<MainService> logger,
            IDatabase cache,
            IConfiguration configuration,
            IGameSalesRepository repo
        )
        {
            _logger = logger;
            _cache = cache;
            _configuration = configuration;
            _repo = repo;
        }

        public override async Task<PingReply> Ping(PingRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new PingReply { Pong = false });
        }

        public override async Task<BulkStoreGameSalesReply> BulkStoreGameSales(BulkStoreGameSalesRequest request, ServerCallContext context)
        {
            var gameSales = request.Items.Select(g => new Models.GameSale
            {
                EuSales = g.EuSales,
                Genre = g.Genre,
                GlobalSales = g.GlobalSales,
                Id = Nanoid.Nanoid.Generate(size: 32),
                JpSales = g.JpSales,
                Name = g.Name,
                NaSales = g.NaSales,
                OtherSales = g.OtherSales,
                Platform = g.Platform,
                Publisher = g.Publisher,
                Rank = g.Rank,
                RegisteredAt = DateTime.UtcNow,
                Year = g.Year,
            }).ToList();

            await _repo.SaveBulkGameSalesAsync(gameSales);

            var reply = new BulkStoreGameSalesReply { };
            var replyItems = new List<GameSale>(gameSales.Count);
            foreach (var item in gameSales)
            {
                replyItems.Add(new GameSale
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
            }
            reply.Items.AddRange(replyItems);

            return await Task.FromResult(reply);
        }

        public override async Task<SearchGameSalesByNameReply> SearchGameSalesByName(SearchGameSalesByNameRequest request, ServerCallContext context)
        {
            var reply = new SearchGameSalesByNameReply { };
            reply.Items.AddRange((await _repo.SearchGameByName(request.Name)).Select(item => new GameSale
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
            }));
            return reply;
        }
    }

}
