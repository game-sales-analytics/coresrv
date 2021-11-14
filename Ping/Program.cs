using System;
using Grpc.Net.Client;
using GSA.Services;


if (args.Length != 1)
{
    Console.WriteLine("server address argument is required");
    Environment.Exit(1);
}

var serverAddress = args[0];

Console.WriteLine("using server address: " + serverAddress);

try
{
    using var channel = GrpcChannel.ForAddress(serverAddress);
    var client = new CoreService.CoreServiceClient(channel);
    var reply = client.Ping(new PingRequest { });
    Console.WriteLine("pong: " + reply.Pong);
    Environment.Exit(0);
}
catch (System.UriFormatException)
{
    Console.WriteLine("invalid server address provided");
    Environment.Exit(1);
}
catch (System.Exception)
{
    Console.WriteLine("failed connecting to server");
    Environment.Exit(2);
}
