using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Generic;

public class PlayerRelationshipsWriter : NetworkPacketWriter
{
    public PlayerRelationshipsWriter(long playerId, ICollection<PlayerRelationship> relationships, ICollection<PlayerFriendship> friendships)
    {
        WriteShort(ServerPacketId.PlayerRelationships);
        WriteLong(playerId);
        WriteInteger(relationships.Count);

        foreach (var relationship in relationships)
        {
            var friend = friendships
                .FirstOrDefault(x => x.OriginPlayerId == relationship.TargetPlayerId || x.TargetPlayerId == relationship.TargetPlayerId);

            if (friend == null)
            {
                continue;
            }
            
            WriteInteger((int) relationship.TypeId);
            WriteInteger(relationships.Count(x => x.TypeId == relationship.TypeId));
            WriteLong(relationship.TargetPlayerId);
            WriteString(friend.TargetPlayer.Username);
            WriteString(friend.TargetPlayer.AvatarData.FigureCode);
        }
    }
}