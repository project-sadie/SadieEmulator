using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players.Friendships;

public class PlayerAcceptFriendRequestEventParser : INetworkPacketEventParser
{
    public int Amount { get; private set; }
    public List<int> Ids { get; } = [];

    public void Parse(INetworkPacketReader reader)
    {
        Amount = reader.ReadInteger();

        for (var i = 0; i < Amount; i++)
        {
            Ids.Add(reader.ReadInteger());
        }
    }
}