using Microsoft.EntityFrameworkCore;
using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Database.Models.Furniture;
using Sadie.Database.Models.Navigator;

namespace Sadie.Database;

public class SadieContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<NavigatorCategoryDto> NavigatorCategories { get; set; }
    public DbSet<NavigatorTabDto> NavigatorTabs { get; set; }
    public DbSet<FurnitureItemDto> FurnitureItems { get; set; }
    public DbSet<CatalogItemDto> CatalogItems { get; set; }
    public DbSet<CatalogPageDto> CatalogPages { get; set; }
    public DbSet<CatalogFrontPageItemDto> CatalogFrontPageItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NavigatorCategoryDto>()
            .HasOne<NavigatorTabDto>(e => e.Tab)
            .WithMany(g => g.Categories)
            .HasForeignKey(s => s.TabId);

        modelBuilder.Entity<FurnitureItemDto>()
            .Property(e => e.Type)
            .HasConversion<string>();

        modelBuilder.Entity<CatalogFrontPageItemDto>()
            .Property(e => e.Type)
            .HasConversion<int>();
    }
}