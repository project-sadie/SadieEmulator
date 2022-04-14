using Sadie.Game.Players.Friendships;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Generic;

public class PlayerRelationshipsWriter : NetworkPacketWriter
{
    public PlayerRelationshipsWriter(long playerId, Dictionary<int, List<PlayerFriendshipData>> relations)
    {
        WriteShort(ServerPacketId.PlayerRelationships);
        WriteLong(playerId);
        WriteInteger(relations.Values.Count(x => x.Count > 0));

        var i = 0;
        var random = new Random();
        
        foreach (var currentRelations in relations.Values)
        {
            if (currentRelations.Count < 1)
            {
                continue;
            }
            
            i++;
            
            var index = random.Next(currentRelations.Count);
            var randomRelation = currentRelations[index];
            
            WriteInteger(i);
            WriteInteger(random.Next(currentRelations.Count));
            WriteLong(randomRelation.Id);
            WriteString(randomRelation.Username);
            WriteString(randomRelation.FigureCode);
        }
    }
}