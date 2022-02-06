namespace Sadie.Networking.Packets.Server.HotelView;

internal class HotelViewBonusRareWriter : NetworkPacketWriter
{
    internal HotelViewBonusRareWriter() : base(ServerPacketId.HotelViewBonusRare)
    {
        WriteString("throne");
        WriteInt(0);
        WriteInt(1000);
        WriteInt(0); // ?
    }
}