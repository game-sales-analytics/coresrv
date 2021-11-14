using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GSA.Services;

namespace App;

public class MainService : CoreService.CoreServiceBase
{
    private readonly ILogger<MainService> _logger;

    private readonly IDatabase _cache;

    private readonly IConfiguration _configuration;

    public MainService(
        ILogger<MainService> logger,
        IDatabase cache,
        IConfiguration configuration
    )
    {
        _logger = logger;
        _cache = cache;
        _configuration = configuration;
    }

    public override async Task<PingReply> Ping(PingRequest request, ServerCallContext context)
    {
        return await Task.FromResult(new PingReply { Pong = true });
    }

    public override async Task<BulkStoreGameSalesReply> BulkStoreGameSales(BulkStoreGameSalesRequest request, ServerCallContext context)
    {
        var value = await _cache.StringGetAsync("prev_id");
        if (value.IsNullOrEmpty)
        {
            value = Nanoid.Nanoid.Generate(size: 64);
            await _cache.StringSetAsync("prev_id", value);
        }

        var reply = new BulkStoreGameSalesReply { };
        reply.Items.Add(
            new GameSale
            {
                EuSales = 29.02F,
                Genre = "Sports",
                GlobalSales = 82.74F,
                OtherSales = 8.46F,
                JpSales = 3.77F,
                Id = value,
                Name = "Wii Sports",
                Rank = 1,
                Publisher = "Wii",
                Year = 2006,
                RegisteredAt = Timestamp.FromDateTime(DateTime.UtcNow),
                Platform = "Nintendo",
                NaSales = 41.49F,
            }
        );

        return await Task.FromResult(reply);
    }
}
