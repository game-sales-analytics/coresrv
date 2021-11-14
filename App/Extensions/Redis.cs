using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace App.Extensions
{
    public static class Redis
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, ConfigurationManager configuration)
        {
            var redisHost = configuration["REDIS_HOST"];
            if (string.IsNullOrWhiteSpace(redisHost))
            {
                Console.WriteLine("'REDIS_HOST' variable must exist and have a value.");
                Environment.Exit(1);
            }

            var redisPort = configuration["REDIS_PORT"];
            if (string.IsNullOrWhiteSpace(redisPort))
            {
                Console.WriteLine("'REDIS_PORT' variable must exist and have a value.");
                Environment.Exit(1);
            }

            var redisPassword = configuration["REDIS_PASSWORD"];
            if (string.IsNullOrWhiteSpace(redisPort))
            {
                Console.WriteLine("'REDIS_PASSWORD' variable must exist and have a value.");
                Environment.Exit(1);
            }

            var multiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { $"{redisHost}:{redisPort}" },
                Password = redisPassword,
            });

            services.AddSingleton<IDatabase>(multiplexer.GetDatabase());

            return services;
        }
    }
}
