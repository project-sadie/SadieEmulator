using Sadie.Shared;

namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomHeightMap : NetworkPacketWriter
{
    internal RoomHeightMap() : base(ServerPacketIds.RoomHeightMap)
    {
        WriteBoolean(true);
        WriteInt(-1);
        WriteString(SadieConstants.MockHeightmap.Replace("\r\n", "\r"));
    }
}