using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Coresrv;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<CoreService>();

app.Run();
