using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Players;

namespace Sadie.Db.Configurations.Players;

public class PlayerDataConfiguration : IEntityTypeConfiguration<PlayerData>
{
    public void Configure(EntityTypeBuilder<PlayerData> entity)
    {
        entity.Property(p => p.RespectPoints).HasDefaultValue(15);
        entity.Property(p => p.RespectPointsPet).HasDefaultValue(15);
        entity.Property(p => p.AchievementScore).HasDefaultValue(15);
        entity.Property(p => p.AllowFriendRequests).HasDefaultValue(true);
        entity.Property(p => p.IsOnline).HasDefaultValue(false);
    }
}