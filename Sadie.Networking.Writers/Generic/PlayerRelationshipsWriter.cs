using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Relationships;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Generic;

public class PlayerRelationshipsWriter : NetworkPacketWriter
{
    public PlayerRelationshipsWriter(long playerId, List<PlayerRelationship> relationships, List<PlayerFriendship> friendships)
    {
        WriteShort(ServerPacketId.PlayerRelationships);
        WriteLong(playerId);
        WriteInteger(relationships.Count);

        foreach (var relationship in relationships)
        {
            var friend = friendships
                .FirstOrDefault(x => x.OriginId == relationship.TargetPlayerId || x.TargetId == relationship.TargetPlayerId);

            if (friend == null)
            {
                continue;
            }
            
            WriteInteger((int) relationship.Type);
            WriteInteger(relationships.Count(x => x.Type == relationship.Type));
            WriteLong(relationship.TargetPlayerId);
            WriteString(friend.TargetData.Username);
            WriteString(friend.TargetData.FigureCode);
        }
    }
}