using Sadie.Shared;

namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomHeightMapWriter : NetworkPacketWriter
{
    internal RoomHeightMapWriter() : base(ServerPacketId.RoomHeightMap)
    {
        WriteBoolean(true);
        WriteInt(-1);
        WriteString(SadieConstants.MockHeightmap.Replace("\r\n", "\r"));
    }
}