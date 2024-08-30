using Sadie.API.Networking;
using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Generic;

[PacketId(ServerPacketId.PlayerRelationships)]
public class PlayerRelationshipsWriter : AbstractPacketWriter
{
    public required long PlayerId { get; init; }
    public required ICollection<PlayerRelationship> Relationships { get; init; }

    public override void OnConfigureRules()
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