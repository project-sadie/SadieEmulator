using Microsoft.EntityFrameworkCore;
using Sadie.Database.Models.Navigator;

namespace Sadie.Database.Data;

public class SadieContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<NavigatorCategoryEntity> NavigatorCategories { get; set; }
    public DbSet<NavigatorTabEntity> NavigatorTabs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NavigatorCategoryEntity>()
            .HasOne<NavigatorTabEntity>(s => s.Tab)
            .WithMany(g => g.Categories)
            .HasForeignKey(s => s.TabId);
    }
}