using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Rooms;

public class PlayerHomeRoomWriter : NetworkPacketWriter
{
    public PlayerHomeRoomWriter(int homeRoom, int newRoom)
    {
        WriteShort(ServerPacketId.PlayerHomeRoom);
        WriteLong(homeRoom);
        WriteLong(newRoom);
    }
}