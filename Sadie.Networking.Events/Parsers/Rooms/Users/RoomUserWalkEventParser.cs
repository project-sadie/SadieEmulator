using System.Drawing;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Users;

public record RoomUserWalkEventParser : INetworkPacketEventParser
{
    public Point Point { get; private set; }
    
    public void Parse(INetworkPacketReader reader)
    {
        Point = new Point(
            reader.ReadInt(), 
            reader.ReadInt());
    }
}