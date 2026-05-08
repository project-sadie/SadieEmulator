using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Constants;

namespace Sadie.Db.Configurations;

public class ServerPlayerConstantsConfiguration
    : IEntityTypeConfiguration<ServerPlayerConstants>
{
    public void Configure(EntityTypeBuilder<ServerPlayerConstants> entity)
    {
        entity.HasNoKey();
        entity.ToTable("server_player_constants");
    }
}