using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players.Friendships;

[PacketId(ServerPacketId.PlayerRemoveFriends)]
public class PlayerRemoveFriendsWriter : AbstractPacketWriter
{
    public required int Unknown1 { get; init; }
    public required List<int> PlayerIds { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(PlayerIds))!, writer =>
        {
            foreach (var playerId in PlayerIds)
            {
                writer.WriteInteger(-1);
                writer.WriteInteger(playerId);
            }
        });
    }
}