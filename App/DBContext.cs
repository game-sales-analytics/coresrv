using Microsoft.EntityFrameworkCore;
using App.DB.Models;

namespace App.DB
{
    public class GameSalesContext : DbContext
    {
        public GameSalesContext(DbContextOptions<GameSalesContext> options) : base(options)
        { }

        public DbSet<GameSale> GameSales { get; init; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameSale>()
                .ToTable("game_sales")
                .HasKey(g => g.Id);
            modelBuilder.Entity<GameSale>()
                .Property(g => g.Id)
                .ValueGeneratedNever()
                .HasMaxLength(32)
                .IsRequired(true)
                .HasColumnType("character(32)")
                .IsFixedLength(true);
            modelBuilder.Entity<GameSale>()
                .HasIndex(g => g.Id, "GAME_SALE_ID_KEY")
                .IsUnique(true);
            modelBuilder.Entity<GameSale>()
                .Property(g => g.Name)
                .IsRequired(true)
                .HasMaxLength(64)
                .HasColumnType("character varying(64)")
                .HasComment("Game name");
            modelBuilder.Entity<GameSale>()
                .Property(g => g.Genre)
                .IsRequired(true)
                .HasMaxLength(32)
                .HasColumnType("character varying(32)")
                .HasComment("Game genre");
            modelBuilder.Entity<GameSale>()
                .HasIndex(g => g.Genre, "GAME_SALE_GENRE_IDX")
                .IsUnique(false);
            modelBuilder.Entity<GameSale>()
                .Property(g => g.Platform)
                .IsRequired(true)
                .HasMaxLength(32)
                .HasColumnType("character varying(32)")
                .HasComment("Game published platform");
            modelBuilder.Entity<GameSale>()
                .HasIndex(g => g.Platform, "GAME_SALE_PLATFORM_IDX")
                .IsUnique(false);
            modelBuilder.Entity<GameSale>()
                .Property(g => g.Publisher)
                .IsRequired(true)
                .HasMaxLength(32)
                .HasColumnType("character varying(32)")
                .HasComment("Game publisher name");
            modelBuilder.Entity<GameSale>()
                .HasIndex(g => g.Publisher, "GAME_SALE_PUBLISHER_IDX")
                .IsUnique(false);
            modelBuilder.Entity<GameSale>()
                .Property(g => g.Year)
                .IsRequired(true)
                .HasColumnType("smallserial");
            modelBuilder.Entity<GameSale>()
                .HasIndex(g => g.Year, "GAME_SALE_YEAR_IDX")
                .IsUnique(false);
            modelBuilder.Entity<GameSale>()
                .Property(g => g.EuSales)
                .IsRequired(true)
                .HasColumnType("real");
            modelBuilder.Entity<GameSale>()
                .Property(g => g.JpSales)
                .IsRequired(true)
                .HasColumnType("real");
            modelBuilder.Entity<GameSale>()
                .Property(g => g.NaSales)
                .IsRequired(true)
                .HasColumnType("real");
            modelBuilder.Entity<GameSale>()
                .Property(g => g.OtherSales)
                .IsRequired(true)
                .HasColumnType("real");
            modelBuilder.Entity<GameSale>()
                .Property(g => g.GlobalSales)
                .IsRequired(true)
                .HasColumnType("real");
            modelBuilder.Entity<GameSale>()
                .Property(g => g.Rank)
                .IsRequired(true)
                .HasColumnType("serial");
            modelBuilder.Entity<GameSale>()
                .Property(g => g.RegisteredAt)
                .IsRequired(true)
                .HasColumnType("timestamp with time zone");
            modelBuilder.Entity<GameSale>()
                .HasGeneratedTsVectorColumn(
                    p => p.SearchVector,
                    "english",
                    p => new { p.Name }
                )
                .HasIndex(p => p.SearchVector)
                .HasMethod("GIN");
        }
    }
}
