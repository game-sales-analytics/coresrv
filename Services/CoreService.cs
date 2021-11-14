using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GSA.Services;

namespace Coresrv;

public class CoreService : GSA.Services.CoreService.CoreServiceBase
{
    private readonly ILogger<CoreService> _logger;

  	private readonly IDistributedCache _distributedCache;

    public CoreService(
        ILogger<CoreService> logger,
        IDistributedCache distributedCache
    ) {
        _logger = logger;
        _distributedCache = distributedCache;
    }

    public override async Task<BulkStoreGameSalesReply> BulkStoreGameSales(BulkStoreGameSalesRequest request, ServerCallContext context)
    {
        var prevId = await _distributedCache.GetStringAsync("prev_id");
        if (string.IsNullOrEmpty(prevId)) {
            prevId = Nanoid.Nanoid.Generate(size: 64);
            await _distributedCache.SetStringAsync("prevId", prevId);
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
                    Id = Nanoid.Nanoid.Generate(size: 64),
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
