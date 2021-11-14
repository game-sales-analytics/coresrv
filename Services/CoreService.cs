using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GSA.Services;

namespace Coresrv;

public class CoreService : GSA.Services.CoreService.CoreServiceBase
{
    private readonly ILogger<CoreService> _logger;

    public CoreService(ILogger<CoreService> logger) => _logger = logger;

    public override async Task<BulkStoreGameSalesReply> BulkStoreGameSales(BulkStoreGameSalesRequest request, ServerCallContext context)
    {
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
