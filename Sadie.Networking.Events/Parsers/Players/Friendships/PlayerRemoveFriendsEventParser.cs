using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players.Friendships;

public class PlayerRemoveFriendsEventParser : INetworkPacketEventParser
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