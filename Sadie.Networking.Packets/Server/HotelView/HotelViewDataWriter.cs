namespace Sadie.Networking.Packets.Server.HotelView;

public class HotelViewDataWriter : NetworkPacketWriter
{
    public HotelViewDataWriter(string key, string value) : base(ServerPacketId.HotelViewData)
    {
        WriteString(key);
        WriteString(value);
    }
}