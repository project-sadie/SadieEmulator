using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players.Friendships;

[PacketId(ServerPacketId.PlayerRemoveFriends)]
public class PlayerRemoveFriendsWriter : AbstractPacketWriter
{
    public required int Unknown1 { get; init; }
    public required List<int> PlayerIds { get; init; }

    public override void OnConfigureRules()
    {
        Override(PropertyHelper<PlayerRemoveFriendsWriter>.GetProperty(x => x.PlayerIds), writer =>
        {
            foreach (var playerId in PlayerIds)
            {
                writer.WriteInteger(-1);
                writer.WriteInteger(playerId);
            }
        });
    }
}