using Sadie.Networking.Packets;

namespace Sadie.Networking.Writers.HotelView;

public class HotelViewBonusRareWriter : NetworkPacketWriter
{
    public HotelViewBonusRareWriter() : base(ServerPacketId.HotelViewBonusRare)
    {
        WriteString("throne");
        WriteInt(0);
        WriteInt(1000);
        WriteInt(0); // ?
    }
}