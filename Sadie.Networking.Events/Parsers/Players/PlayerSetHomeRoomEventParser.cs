using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players;

public class PlayerSetHomeRoomEventParser : INetworkPacketEventParser
{
    public int RoomId { get; set; }
    
    public void Parse(INetworkPacketReader reader)
    {
        RoomId = reader.ReadInt();
    }
}