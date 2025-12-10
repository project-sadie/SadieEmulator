using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Rooms;

namespace Sadie.Db.Configurations.Rooms;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> entity)
    {
        entity.HasOne(r => r.PaintSettings)
            .WithOne(p => p.Room)
            .HasForeignKey<RoomPaintSettings>(p => p.RoomId);

        entity.HasMany(r => r.FurnitureItems)
            .WithOne(f => f.Room)
            .HasForeignKey(f => f.RoomId);

        entity.Navigation(r => r.Settings).AutoInclude();
    }
}