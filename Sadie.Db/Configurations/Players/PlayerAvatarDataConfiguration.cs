using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Core.Shared.Helpers;
using Sadie.Db.Models.Players;

namespace Sadie.Db.Configurations.Players;

public class PlayerAvatarDataConfiguration : IEntityTypeConfiguration<PlayerAvatarData>
{
    public void Configure(EntityTypeBuilder<PlayerAvatarData> entity)
    {
        entity.Property(p => p.Gender)
            .HasConversion(
                v => EnumHelpers.GetEnumDescription(v),
                v => EnumHelpers.GetEnumValueFromDescription<PlayerAvatarGender>(v.ToUpper()));

        entity.Property(p => p.ChatBubbleId)
            .HasDefaultValue(ChatBubble.Default);
    }
}