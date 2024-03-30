using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms;

public class RoomLoadedParser
{
    public int RoomId { get; private set; }
    public string Password { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        RoomId = reader.ReadInteger();
        Password = reader.ReadString();
    }
}