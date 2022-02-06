namespace Sadie.Networking.Packets.Server.HotelView;

internal class HotelViewDataWriter : NetworkPacketWriter
{
    internal HotelViewDataWriter(string key, string value) : base(ServerPacketId.HotelViewData)
    {
        WriteString(key);
        WriteString(value);
    }
}