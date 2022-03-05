using Sadie.Game.Players.Friendships;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerFriendRequestsWriter : NetworkPacketWriter
{
    public PlayerFriendRequestsWriter(List<PlayerFriendshipData> requests)
    {
        WriteShort(ServerPacketId.PlayerFriendRequests);
        WriteInt(requests.Count);
        WriteInt(requests.Count);

        foreach (var request in requests)
        {
            WriteLong(request.Id);
            WriteString(request.Username);
            WriteString(request.FigureCode);
        }
    }
}