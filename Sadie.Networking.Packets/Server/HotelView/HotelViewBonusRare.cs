namespace Sadie.Networking.Packets.Server.HotelView;

public class HotelViewBonusRare : NetworkPacketWriter
{
    public HotelViewBonusRare() : base(ServerPacketIds.HotelViewBonusRare)
    {
        WriteString("throne");
        WriteInt(0);
        WriteInt(1000);
        WriteInt(0); // ?
    }
}