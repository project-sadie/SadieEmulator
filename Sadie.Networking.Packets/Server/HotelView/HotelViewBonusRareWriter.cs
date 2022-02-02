namespace Sadie.Networking.Packets.Server.HotelView;

public class HotelViewBonusRareWriter : NetworkPacketWriter
{
    public HotelViewBonusRareWriter() : base(ServerPacketId.HotelViewBonusRare)
    {
        // TODO: Pass structure in 
        
        WriteString("throne");
        WriteInt(0);
        WriteInt(1000);
        WriteInt(0); // ?
    }
}