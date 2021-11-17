using System;


namespace App.Models
{
    public record GameSale
    {
        public float EuSales { get; init; } = default!;

        public string Genre { get; init; } = default!;

        public float GlobalSales { get; init; } = default!;

        public float OtherSales { get; init; } = default!;

        public float JpSales { get; init; } = default!;

        public string Id { get; init; } = default!;

        public string Name { get; init; } = default!;

        public ulong Rank { get; init; } = default!;

        public string Publisher { get; init; } = default!;

        public uint Year { get; init; } = default!;

        public DateTime RegisteredAt { get; init; } = default!;

        public string Platform { get; init; } = default!;

        public float NaSales { get; init; } = default!;
    }
}