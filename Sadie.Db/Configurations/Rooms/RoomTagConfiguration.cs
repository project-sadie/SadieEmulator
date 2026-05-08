using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Rooms;

namespace Sadie.Db.Configurations.Rooms;

public class RoomTagConfiguration : IEntityTypeConfiguration<RoomTag>
{
    public void Configure(EntityTypeBuilder<RoomTag> entity)
    {
        entity.ToTable("room_tags");
    }
}