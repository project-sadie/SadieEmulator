namespace Sadie.Networking.Packets.Server.Players.Rooms;

public class PlayerHomeRoomPacket : NetworkPacketWriter
{
    public PlayerHomeRoomPacket(long homeRoom, int newRoom) : base(ServerPacketIds.PlayerHomeRoom)
    {
        WriteLong(homeRoom);
        WriteInt(newRoom);
    }
}