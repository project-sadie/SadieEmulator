using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Constants;

namespace Sadie.Db.Configurations;

public class ServerRoomConstantsConfiguration
    : IEntityTypeConfiguration<ServerRoomConstants>
{
    public void Configure(EntityTypeBuilder<ServerRoomConstants> entity)
    {
        entity.HasNoKey();
        entity.ToTable("server_room_constants");
    }
}