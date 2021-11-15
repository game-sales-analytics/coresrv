using System;

namespace App.DB.Models
{
    public record GameSale
    {
        public float EuSales { get; init; }

        public string Genre { get; init; }

        public float GlobalSales { get; init; }

        public float OtherSales { get; init; }

        public float JpSales { get; init; }

        public string Id { get; init; }

        public string Name { get; init; }

        public ulong Rank { get; init; }

        public string Publisher { get; init; }

        public uint Year { get; init; }

        public DateTime RegisteredAt { get; init; }

        public string Platform { get; init; }

        public float NaSales { get; init; }
    };
}