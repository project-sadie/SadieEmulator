using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Rights;

public class RoomRemoveUserRightsEventParser : INetworkPacketEventParser
{
    public int Amount { get; private set; }
    public List<int> Ids { get; } = [];

    public void Parse(INetworkPacketReader reader)
    {
        Amount = reader.ReadInt();

        for (var i = 0; i < Amount; i++)
        {
            Ids.Add(reader.ReadInt());
        }
    }
}