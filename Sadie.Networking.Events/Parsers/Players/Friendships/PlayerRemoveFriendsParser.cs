using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players.Friendships;

public class PlayerRemoveFriendsParser
{
    public int Amount { get; private set; }
    public List<int> Ids { get; } = new();

    public void Parse(INetworkPacketReader reader)
    {
        Amount = reader.ReadInteger();

        for (var i = 0; i < Amount; i++)
        {
            Ids.Add(reader.ReadInteger());
        }
    }
}