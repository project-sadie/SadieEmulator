using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Players;

namespace Sadie.Db.Configurations.Players;

public class PlayerFriendshipConfiguration : IEntityTypeConfiguration<PlayerFriendship>
{
    public void Configure(EntityTypeBuilder<PlayerFriendship> entity)
    {
        entity.ToTable("player_friendships");

        entity.HasKey(x => x.Id);

        entity.HasOne(x => x.OriginPlayer)
            .WithMany(p => p.OutgoingFriendships)
            .HasForeignKey(x => x.OriginPlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.TargetPlayer)
            .WithMany(p => p.IncomingFriendships)
            .HasForeignKey(x => x.TargetPlayerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}