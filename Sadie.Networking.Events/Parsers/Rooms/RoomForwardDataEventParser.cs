using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms;

public class RoomForwardDataEventParser : INetworkPacketEventParser
{
    public int RoomId { get; private set; }
    public int Unknown1 { get; private set; }
    public int Unknown2 { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        RoomId = reader.ReadInt();
        Unknown1 = reader.ReadInt();
        Unknown2 = reader.ReadInt();
    }
}