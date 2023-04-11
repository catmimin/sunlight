﻿using Microsoft.EntityFrameworkCore;
using SunLight.Database.Game.Item;

namespace SunLight.Database.Game;

public class ItemDbContext : DbContext
{
    public DbSet<AwardM> AwardM { get; init; }
    public DbSet<BackgroundM> BackgroundM { get; init; }
    public DbSet<LiveSeM> LiveSeM { get; init; }
    public DbSet<LiveNotesIconM> LiveNotesIconM { get; init; }

    public ItemDbContext(DbContextOptions<ItemDbContext> dbContextOptions) : base(dbContextOptions)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Assets/item.db_").UseSnakeCaseNamingConvention();
    }
}