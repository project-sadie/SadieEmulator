using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Rooms;

namespace Sadie.Db.Configurations.Rooms;

public class RoomPaintSettingsConfiguration : IEntityTypeConfiguration<RoomPaintSettings>
{
    public void Configure(EntityTypeBuilder<RoomPaintSettings> entity)
    {
        entity.Property(p => p.FloorPaint).HasDefaultValue("0.0");
        entity.Property(p => p.WallPaint).HasDefaultValue("0.0");
        entity.Property(p => p.LandscapePaint).HasDefaultValue("0.0");
    }
}
