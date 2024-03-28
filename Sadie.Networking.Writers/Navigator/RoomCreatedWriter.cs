using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class RoomCreatedWriter : NetworkPacketWriter
{
    public RoomCreatedWriter(int id, string name)
    {
        WriteShort(ServerPacketId.RoomCreated);
        WriteInteger(id);
        WriteString(name);
    }
}