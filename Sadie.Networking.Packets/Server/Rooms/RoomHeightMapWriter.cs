using Sadie.Shared;

namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomHeightMapWriter : NetworkPacketWriter
{
    internal RoomHeightMapWriter(bool unknown1, int unknown2, string unknown3) : base(ServerPacketId.RoomHeightMap)
    {
        WriteBoolean(unknown1);
        WriteInt(unknown2);
        WriteString(unknown3);
    }
}