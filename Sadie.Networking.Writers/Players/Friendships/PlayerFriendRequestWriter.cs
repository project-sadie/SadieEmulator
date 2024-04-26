using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friendships;

public class PlayerFriendRequestWriter : AbstractPacketWriter
{
    public PlayerFriendRequestWriter(int id, string username, string figureCode)
    {
        WriteShort(ServerPacketId.PlayerFriendRequest);
        WriteInteger(id);
        WriteString(username);
        WriteString(figureCode);
    }
}