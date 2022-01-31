namespace Sadie.Networking.Packets.Server.HotelView;

public class HotelViewDataWriter : NetworkPacketWriter
{
    public HotelViewDataWriter(string key, string value) : base(ServerPacketIds.HotelViewData)
    {
        Console.WriteLine($"Sending [ key: '{key}', value: '{value}' ] via hotel view");
        
        WriteString(key);
        WriteString(value);
    }
}