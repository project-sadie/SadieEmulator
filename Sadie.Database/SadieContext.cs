using Microsoft.EntityFrameworkCore;
using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Database.Models.Furniture;
using Sadie.Database.Models.Navigator;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted;

namespace Sadie.Database;

public class SadieContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<NavigatorCategory> NavigatorCategories { get; set; }
    public DbSet<NavigatorTab> NavigatorTabs { get; set; }
    public DbSet<FurnitureItem> FurnitureItems { get; set; }
    public DbSet<CatalogItem> CatalogItems { get; set; }
    public DbSet<CatalogPage> CatalogPages { get; set; }
    public DbSet<CatalogFrontPageItem> CatalogFrontPageItems { get; set; }
    public DbSet<RoomCategory> RoomCategories { get; set; }
    public DbSet<RoomChatMessage> RoomChatMessages { get; set; }
    public DbSet<RoomFurnitureItem> RoomFurnitureItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NavigatorCategory>()
            .HasOne<NavigatorTab>(e => e.Tab)
            .WithMany(g => g.Categories)
            .HasForeignKey(s => s.TabId);

        modelBuilder.Entity<FurnitureItem>()
            .Property(e => e.Type)
            .HasConversion(
                v => v.ToString(),
                v => EnumHelpers.GetEnumValueFromDescription<FurnitureItemType>(v));

        modelBuilder.Entity<CatalogFrontPageItem>()
            .Property(e => e.Type)
            .HasConversion<int>();
        
        modelBuilder.Entity<CatalogFrontPageItem>()
            .Property(e => e.Type)
            .HasColumnName("type_id");

        modelBuilder.Entity<CatalogItem>()
            .HasMany(c => c.FurnitureItems)
            .WithMany(x => x.CatalogItems);
    }
}