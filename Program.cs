using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Coresrv;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddDistributedRedisCache(option =>
	{
		option.Configuration = "127.0.0.1";
		option.InstanceName = "master";
	});

var app = builder.Build();

app.MapGrpcService<CoreService>();

app.Run();
