using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models;

namespace Sadie.Db.Configurations;

public class HandItemConfiguration : IEntityTypeConfiguration<HandItem>
{
    public void Configure(EntityTypeBuilder<HandItem> entity)
    {
        entity.ToTable("hand_items");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255);

        entity
            .HasMany(x => x.FurnitureItems)
            .WithMany(x => x.HandItems)
            .UsingEntity<Dictionary<string, object>>(
                "furniture_item_hand_item",
                j => j
                    .HasOne<Sadie.Db.Models.Furniture.FurnitureItem>()
                    .WithMany()
                    .HasForeignKey("furniture_items_id")
                    .HasPrincipalKey("Id"),
                j => j
                    .HasOne<HandItem>()
                    .WithMany()
                    .HasForeignKey("hand_items_id")
                    .HasPrincipalKey("Id")
            );
    }
}