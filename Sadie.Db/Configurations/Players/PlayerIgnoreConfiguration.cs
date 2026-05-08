using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Players;

namespace Sadie.Db.Configurations.Players;

public class PlayerIgnoreConfiguration : IEntityTypeConfiguration<PlayerIgnore>
{
    public void Configure(EntityTypeBuilder<PlayerIgnore> entity)
    {
        entity.ToTable("player_ignores");

        entity.HasKey(x => x.Id);

        entity.HasOne<Player>()
            .WithMany(p => p.OutgoingIgnores)
            .HasForeignKey(x => x.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne<Player>()
            .WithMany(p => p.IncomingIgnores)
            .HasForeignKey(x => x.TargetPlayerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}