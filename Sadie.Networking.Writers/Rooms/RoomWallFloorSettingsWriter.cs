using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomWallFloorSettingsWriter : NetworkPacketWriter
{
    public RoomWallFloorSettingsWriter(bool hideWalls, int wallThickness, int floorThickness)
    {
        WriteShort(ServerPacketId.RoomWallFloorSettings);
        WriteBool(hideWalls);
        WriteInteger(wallThickness);
        WriteInteger(floorThickness);
    }
}