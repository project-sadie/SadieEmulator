using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Core.Enums.Game.Rooms;
using Sadie.Db.Models.Rooms;

namespace Sadie.Db.Configurations.Rooms;

public class RoomSettingsConfiguration : IEntityTypeConfiguration<RoomSettings>
{
    public void Configure(EntityTypeBuilder<RoomSettings> entity)
    {
        entity.Property(p => p.WalkDiagonal).HasDefaultValue(true);
        entity.Property(p => p.AccessType).HasDefaultValue(RoomAccessType.Open);
        entity.Property(p => p.WhoCanMute).HasDefaultValue(0);
        entity.Property(p => p.WhoCanKick).HasDefaultValue(0);
        entity.Property(p => p.WhoCanBan).HasDefaultValue(0);
        entity.Property(p => p.AllowPets).HasDefaultValue(true);
        entity.Property(p => p.CanPetsEat).HasDefaultValue(true);
        entity.Property(p => p.HideWalls).HasDefaultValue(false);
        entity.Property(p => p.WallThickness).HasDefaultValue(0);
        entity.Property(p => p.FloorThickness).HasDefaultValue(0);
        entity.Property(p => p.CanUsersOverlap).HasDefaultValue(false);
    }
}
