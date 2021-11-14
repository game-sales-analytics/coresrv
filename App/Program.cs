using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using App;
using App.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddRedis(builder.Configuration);
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<MainService>();

app.Run();
