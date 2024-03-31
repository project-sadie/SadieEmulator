using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players.Friendships;

public class PlayerDeclineFriendRequestEventParser : INetworkPacketEventParser
{
    public bool DeclineAll { get; private set; }
    public int Amount { get; private set; }
    public List<int> Ids { get; } = [];

    public void Parse(INetworkPacketReader reader)
    {
        DeclineAll = reader.ReadBool();

        if (DeclineAll)
        {
            return;
        }
        
        Amount = reader.ReadInteger();

        for (var i = 0; i < Amount; i++)
        {
            Ids.Add(reader.ReadInteger());
        }
    }
}