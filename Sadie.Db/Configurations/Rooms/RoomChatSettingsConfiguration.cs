using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Rooms;

namespace Sadie.Db.Configurations.Rooms;

public class RoomChatSettingsConfiguration : IEntityTypeConfiguration<RoomChatSettings>
{
    public void Configure(EntityTypeBuilder<RoomChatSettings> entity)
    {
        entity.Property(p => p.ChatType).HasDefaultValue(0);
        entity.Property(p => p.ChatWeight).HasDefaultValue(1);
        entity.Property(p => p.ChatSpeed).HasDefaultValue(1);
        entity.Property(p => p.ChatDistance).HasDefaultValue(50);
        entity.Property(p => p.ChatProtection).HasDefaultValue(1);
    }
}