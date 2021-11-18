using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GSA.Rpc;
using App.DB.Repository;


namespace App
{
    public class MainService : CoreService.CoreServiceBase
    {
        private readonly ILogger<MainService> _logger;

        private readonly ICacheService cache;

        private readonly IConfiguration _configuration;

        private readonly IGameSalesRepository _repo;

        public MainService(
            ILogger<MainService> logger,
            ICacheService _cache,
            IConfiguration configuration,
            IGameSalesRepository repo
        )
        {
            _logger = logger;
            cache = _cache;
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

            try
            {
                await _repo.SaveBulkGameSalesAsync(gameSales, context.CancellationToken);
            }
            catch (App.DB.DuplicateRecordException)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, "duplicate game sale entry exists"));
            }

            await cache.UpdateGamesSalesCacheAsync(context.CancellationToken);

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
            reply.Items.AddRange((await _repo.SearchGameByName(request.Name, context.CancellationToken)).Select(item => new GameSale
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

        public override async Task<GetGameSalesWithMoreSalesInEUThanNAReply> GetGameSalesWithMoreSalesInEUThanNA(GetGameSalesWithMoreSalesInEUThanNARequest request, ServerCallContext context)
        {
            IEnumerable<GameSale> games = new List<GameSale>();

            var cachedResponse = await cache.ReadGamesWithMoreEUSalesThanNASalesAsync();
            if (!cachedResponse.IsNullOrEmpty)
            {
                games = cachedResponse.GameSales;
            }
            else
            {
                games = (await _repo.GetGameSalesWithMoreSalesInEUThanNA(context.CancellationToken)).Select(item => new GameSale
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
            var reply = new GetGameSalesWithMoreSalesInEUThanNAReply { };
            reply.Items.AddRange(games);
            return reply;
        }

        public override async Task<GetGameSalesByRankReply> GetGameSalesByRank(GetGameSalesByRankRequest request, ServerCallContext context)
        {
            var game = await _repo.GetGameSalesByRank(request.Rank, context.CancellationToken);
            if (game == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "game with specified rank does not exist"));
            }

            return new GetGameSalesByRankReply
            {
                GameSale = new GameSale
                {
                    EuSales = game.EuSales,
                    Genre = game.Genre,
                    GlobalSales = game.GlobalSales,
                    Id = game.Id,
                    JpSales = game.JpSales,
                    Name = game.Name,
                    NaSales = game.NaSales,
                    OtherSales = game.OtherSales,
                    Platform = game.Platform,
                    Publisher = game.Publisher,
                    Rank = game.Rank,
                    RegisteredAt = Timestamp.FromDateTime(game.RegisteredAt),
                    Year = game.Year,
                }
            };
        }

        public override async Task<GetTopNGamesOfPlatformsReply> GetTopNGamesOfPlatforms(GetTopNGamesOfPlatformsRequest request, ServerCallContext context)
        {
            var platfromGamesMap = await _repo.GetTopNGamesOfPlatforms(request.N, context.CancellationToken);

            var reply = new GetTopNGamesOfPlatformsReply();
            foreach (var platformGame in platfromGamesMap)
            {
                var platformGames = new GetTopNGamesOfPlatformsReply.Types.GameSales { };
                platformGames.Items.AddRange(platformGame.Value.Select(g => new GameSale
                {
                    EuSales = g.EuSales,
                    Genre = g.Genre,
                    GlobalSales = g.GlobalSales,
                    Id = g.Id,
                    JpSales = g.JpSales,
                    Name = g.Name,
                    NaSales = g.NaSales,
                    OtherSales = g.OtherSales,
                    Platform = g.Platform,
                    Publisher = g.Publisher,
                    Rank = g.Rank,
                    RegisteredAt = Timestamp.FromDateTime(g.RegisteredAt),
                    Year = g.Year,
                }));
                reply.Group.Add(platformGame.Key, platformGames);
            }

            return reply;
        }

        public override async Task<GetTotalGameSalesInYearsRangeByGenreReply> GetTotalGameSalesInYearsRangeByGenre(GetTotalGameSalesInYearsRangeByGenreRequest request, ServerCallContext context)
        {
            var gameSalesByGenres = await _repo.GetTotalGameSalesInYearsRangeByGenre(request.StartYear, request.EndYear, context.CancellationToken);

            var reply = new GetTotalGameSalesInYearsRangeByGenreReply();
            foreach (var item in gameSalesByGenres)
            {
                reply.GenreTotalSales.Add(item.Key, item.Value);
            }

            return reply;
        }

        public override async Task<GetYearlyTotalGameSalesInRangeReply> GetYearlyTotalGameSalesInRange(GetYearlyTotalGameSalesInRangeRequest request, ServerCallContext context)
        {
            var gameSales = await _repo.GetYearlyTotalGameSalesInRange(request.StartYear, request.EndYear, context.CancellationToken);

            var reply = new GetYearlyTotalGameSalesInRangeReply();
            reply.Items.AddRange(gameSales.Select(gs => new GetYearlyTotalGameSalesInRangeReply.Types.TotalYearGameSales { TotalGameSales = gs.Value, Year = gs.Key }));

            return reply;
        }
    }
}
