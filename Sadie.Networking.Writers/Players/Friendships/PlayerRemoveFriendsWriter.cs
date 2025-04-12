using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Friendships;

[PacketId(ServerPacketId.PlayerRemoveFriends)]
public class PlayerRemoveFriendsWriter : AbstractPacketWriter
{
    public required int Unknown1 { get; init; }
    public required ICollection<long> PlayerIds { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(PlayerIds))!, writer =>
        {
            writer.WriteInteger(PlayerIds.Count);
            
            foreach (var playerId in PlayerIds)
            {
                writer.WriteInteger(-1);
                writer.WriteInteger((int) playerId);
            }
        });
    }
}