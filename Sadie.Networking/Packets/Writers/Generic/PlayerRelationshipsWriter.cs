using Sadie.API.DTOs.Player;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Generic;

[PacketId(ServerPacketId.PlayerRelationships)]
public class PlayerRelationshipsWriter : AbstractPacketWriter
{
    public required long PlayerId { get; init; }
    public required ICollection<PlayerRelationshipDto> Relationships { get; init; }

    public override async Task OnConfigureRulesAsync()
    {
        Override(GetType().GetProperty(nameof(Relationships))!, writer =>
        {
            writer.WriteInteger(Relationships.Count);

            foreach (var relationship in Relationships)
            {
                writer.WriteInteger((int) relationship.TypeId);
                writer.WriteInteger(Relationships.Count(x => x.TypeId == relationship.TypeId));
                writer.WriteLong(relationship.TargetPlayerId);
                writer.WriteString(relationship.TargetPlayer.Username);
                writer.WriteString(relationship.TargetPlayer.AvatarData.FigureCode);
            }
        });
    }
}