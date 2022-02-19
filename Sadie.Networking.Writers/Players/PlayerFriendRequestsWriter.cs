using Sadie.Game.Players.Friendships;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerFriendRequestsWriter : NetworkPacketWriter
{
    public PlayerFriendRequestsWriter(List<PlayerFriendshipData> requests) : base(ServerPacketId.PlayerFriendRequests)
    {
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