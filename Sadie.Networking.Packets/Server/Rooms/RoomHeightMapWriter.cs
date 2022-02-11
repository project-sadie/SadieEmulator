namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomHeightMapWriter : NetworkPacketWriter
{
    internal RoomHeightMapWriter(bool unknown1, int wallHeight, string relativeHeightmap) : base(ServerPacketId.RoomHeightMap)
    {
        WriteBoolean(unknown1);
        WriteInt(wallHeight);
        WriteString(relativeHeightmap);
    }
}