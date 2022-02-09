using Sadie.Game.Players.Friendships;

namespace Sadie.Networking.Packets.Server.Players;

internal class PlayerFriendRequestsWriter : NetworkPacketWriter
{
    internal PlayerFriendRequestsWriter(List<PlayerFriendshipData> requests) : base(ServerPacketId.PlayerFriendRequests)
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