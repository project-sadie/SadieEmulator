using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerFriendRequests)]
public class PlayerFriendRequestsWriter : AbstractPacketWriter
{
    public PlayerFriendRequestsWriter(List<Player> requests)
    {
        WriteInteger(requests.Count);
        WriteInteger(requests.Count);

        foreach (var request in requests)
        {
            WriteLong(request.Id);
            WriteString(request.Username);
            WriteString(request.AvatarData.FigureCode);
        }
    }
}