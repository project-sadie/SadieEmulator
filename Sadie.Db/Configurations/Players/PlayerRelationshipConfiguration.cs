using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Players;

namespace Sadie.Db.Configurations.Players;

public class PlayerRelationshipConfiguration : IEntityTypeConfiguration<PlayerRelationship>
{
    public void Configure(EntityTypeBuilder<PlayerRelationship> entity)
    {
        entity.ToTable("player_relationships");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.OriginPlayerId).IsRequired();
        entity.Property(x => x.TargetPlayerId).IsRequired();
        entity.Property(x => x.TypeId).IsRequired();

        entity.HasOne(x => x.OriginPlayer)
            .WithMany(x => x.OriginRelationships)
            .HasForeignKey(x => x.OriginPlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.TargetPlayer)
            .WithMany(x => x.TargetRelationships)
            .HasForeignKey(x => x.TargetPlayerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}