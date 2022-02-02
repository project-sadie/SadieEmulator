namespace Sadie.Networking.Packets.Server.Players.Rooms;

public class PlayerHomeRoomPacket : NetworkPacketWriter
{
    public PlayerHomeRoomPacket(long homeRoom, int newRoom) : base(ServerPacketId.PlayerHomeRoom)
    {
        WriteLong(homeRoom);
        WriteInt(newRoom);
    }
}