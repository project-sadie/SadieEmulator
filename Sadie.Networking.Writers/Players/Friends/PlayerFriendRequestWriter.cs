using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friends;

public class PlayerFriendRequestWriter : NetworkPacketWriter
{
    public PlayerFriendRequestWriter(int id, string username, string figureCode)
    {
        WriteShort(ServerPacketId.PlayerFriendRequest);
        WriteInt(id);
        WriteString(username);
        WriteString(figureCode);
    }
}