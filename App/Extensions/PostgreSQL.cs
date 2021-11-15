using System;
using Microsoft.Extensions.Configuration;

namespace App.Extensions
{
    public static class PostgreSQL
    {
        public static string BuildPostgreSQLConnectionString(ConfigurationManager configuration)
        {
            var pgHost = configuration["POSTGRESQL_HOST"];
            if (string.IsNullOrWhiteSpace(pgHost))
            {
                Console.WriteLine("'POSTGRESQL_HOST' variable must exist and have a value.");
                Environment.Exit(1);
            }

            var pgPort = configuration["POSTGRESQL_PORT"];
            if (string.IsNullOrWhiteSpace(pgPort))
            {
                Console.WriteLine("'POSTGRESQL_PORT' variable must exist and have a value.");
                Environment.Exit(1);
            }

            var pgUser = configuration["POSTGRESQL_USERNAME"];
            if (string.IsNullOrWhiteSpace(pgUser))
            {
                Console.WriteLine("'POSTGRESQL_USERNAME' variable must exist and have a value.");
                Environment.Exit(1);
            }

            var pgPassword = configuration["POSTGRESQL_PASSWORD"];
            if (string.IsNullOrWhiteSpace(pgPassword))
            {
                Console.WriteLine("'POSTGRESQL_PASSWORD' variable must exist and have a value.");
                Environment.Exit(1);
            }

            var pgDB = configuration["POSTGRESQL_DB"];
            if (string.IsNullOrWhiteSpace(pgDB))
            {
                Console.WriteLine("'POSTGRESQL_DB' variable must exist and have a value.");
                Environment.Exit(1);
            }

            var connString = $"Host={pgHost};Port={pgPort};Username={pgUser};Password={pgPassword};Database={pgDB}";
            return connString;
        }
    }
}

