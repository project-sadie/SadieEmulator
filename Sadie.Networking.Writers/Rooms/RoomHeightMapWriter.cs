using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomHeightMapWriter : NetworkPacketWriter
{
    public RoomHeightMapWriter(bool unknown1, int wallHeight, string relativeHeightmap) : base(ServerPacketId.RoomHeightMap)
    {
        WriteBoolean(unknown1);
        WriteInt(wallHeight);
        WriteString(relativeHeightmap);
    }
}