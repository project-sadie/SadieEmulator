using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Rooms;

public class PlayerHomeRoomWriter : AbstractPacketWriter
{
    public PlayerHomeRoomWriter(int homeRoom, int roomIdToEnter)
    {
        WriteShort(ServerPacketId.PlayerHomeRoom);
        WriteLong(homeRoom);
        WriteLong(roomIdToEnter);
    }
}