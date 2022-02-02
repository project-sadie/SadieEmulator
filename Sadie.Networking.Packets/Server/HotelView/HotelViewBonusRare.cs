namespace Sadie.Networking.Packets.Server.HotelView;

public class HotelViewBonusRare : NetworkPacketWriter
{
    public HotelViewBonusRare() : base(ServerPacketId.HotelViewBonusRare)
    {
        WriteString("throne");
        WriteInt(0);
        WriteInt(1000);
        WriteInt(0); // ?
    }
}