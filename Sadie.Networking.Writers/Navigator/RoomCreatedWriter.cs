using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

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