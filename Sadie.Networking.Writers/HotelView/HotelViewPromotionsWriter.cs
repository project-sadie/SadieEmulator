using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.HotelView;

public class HotelViewPromotionsWriter : NetworkPacketWriter
{
    public HotelViewPromotionsWriter()
    {
        WriteInteger(0);
    }
}