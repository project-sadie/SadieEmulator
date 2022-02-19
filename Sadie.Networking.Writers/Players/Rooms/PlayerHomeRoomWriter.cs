using Sadie.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Rooms;

public class PlayerHomeRoomWriter : NetworkPacketWriter
{
    public PlayerHomeRoomWriter(long homeRoom, int newRoom) : base(ServerPacketId.PlayerHomeRoom)
    {
        WriteLong(homeRoom);
        WriteInt(newRoom);
    }
}