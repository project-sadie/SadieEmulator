namespace Sadie.Networking.Packets.Server.Rooms;

public class RoomHeightMapWriter : NetworkPacketWriter
{
    public RoomHeightMapWriter(bool unknown1, int wallHeight, string relativeHeightmap) : base(ServerPacketId.RoomHeightMap)
    {
        WriteBoolean(unknown1);
        WriteInt(wallHeight);
        WriteString(relativeHeightmap);
    }
}