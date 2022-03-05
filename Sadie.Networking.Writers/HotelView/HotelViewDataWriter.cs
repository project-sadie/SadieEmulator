using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.HotelView;

public class HotelViewDataWriter : NetworkPacketWriter
{
    public HotelViewDataWriter(string key, string value)
    {
        WriteShort(ServerPacketId.HotelViewData);
        WriteString(key);
        WriteString(value);
    }
}