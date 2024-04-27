using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Generic;

public class PlayerRelationshipsWriter : AbstractPacketWriter
{
    public PlayerRelationshipsWriter(long playerId, ICollection<PlayerRelationship> relationships)
    {
        WriteShort(ServerPacketId.PlayerRelationships);
        WriteLong(playerId);
        WriteInteger(relationships.Count);

        foreach (var relationship in relationships)
        {
            WriteInteger((int) relationship.TypeId);
            WriteInteger(relationships.Count(x => x.TypeId == relationship.TypeId));
            WriteLong(relationship.TargetPlayerId);
            WriteString(relationship.TargetPlayer.Username);
            WriteString(relationship.TargetPlayer.AvatarData.FigureCode);
        }
    }
}