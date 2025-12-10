using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models;
using Sadie.Db.Models.Players;

namespace Sadie.Db.Configurations.Players;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> entity)
    {
        entity.ToTable("players");

        entity
            .HasMany(r => r.Roles)
            .WithMany(p => p.Players)
            .UsingEntity(
                "player_role",
                l => l.HasOne(typeof(Role))
                    .WithMany()
                    .HasForeignKey("role_id")
                    .HasPrincipalKey(nameof(Role.Id)),
                r => r.HasOne(typeof(Player))
                    .WithMany()
                    .HasForeignKey("player_id")
                    .HasPrincipalKey(nameof(Player.Id)),
                j => j.HasKey("role_id", "player_id")
            );

        entity
            .HasMany(r => r.Groups)
            .WithMany(p => p.Players)
            .UsingEntity(
                "group_player",
                l => l.HasOne(typeof(Group))
                    .WithMany()
                    .HasForeignKey("group_id"),
                r => r.HasOne(typeof(Player))
                    .WithMany()
                    .HasForeignKey("player_id"),
                j => j.HasKey("group_id", "player_id")
            );

        entity
            .Navigation(x => x.AvatarData)
            .AutoInclude();

        entity
            .HasMany<PlayerBan>(x => x.Bans)
            .WithOne(x => x.Player);
    }
}