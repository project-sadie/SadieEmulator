using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.HotelView;

public class HotelViewBonusRareWriter : NetworkPacketWriter
{
    public HotelViewBonusRareWriter()
    {
        WriteShort(ServerPacketId.HotelViewBonusRare);
        WriteString("throne");
        WriteInt(0);
        WriteInt(1000);
        WriteInt(0); // ?
    }
}