using Sadie.Game.Players;

namespace Sadie.Networking.Packets.Server.Players;

internal class PlayerFriendRequestsWriter : NetworkPacketWriter
{
    internal PlayerFriendRequestsWriter(List<PlayerFriendRequest> requests) : base(ServerPacketId.PlayerFriendRequests)
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