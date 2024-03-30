using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.HotelView;

public class HotelViewBonusRareWriter : NetworkPacketWriter
{
    public HotelViewBonusRareWriter()
    {
        WriteShort(ServerPacketId.HotelViewBonusRare);
        WriteString("throne");
        WriteInteger(0);
        WriteInteger(1000);
        WriteInteger(0); // ?
    }
}