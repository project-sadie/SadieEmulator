using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Users;

public record RoomUserLookAtParser
{
    public int X { get; private set; }
    public int Y { get; private set; }
    
    public void Parse(INetworkPacketReader reader)
    {
        X = reader.ReadInteger();
        Y = reader.ReadInteger();
    }
}