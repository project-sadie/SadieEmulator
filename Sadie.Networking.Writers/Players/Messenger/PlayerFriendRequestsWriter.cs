using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Messenger;

public class PlayerFriendRequestsWriter : NetworkPacketWriter
{
    public PlayerFriendRequestsWriter(List<Player> requests)
    {
        WriteShort(ServerPacketId.PlayerFriendRequests);
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