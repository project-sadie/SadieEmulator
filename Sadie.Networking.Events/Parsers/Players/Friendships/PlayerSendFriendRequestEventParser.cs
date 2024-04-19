using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players.Friendships;

public class PlayerSendFriendRequestEventParser : INetworkPacketEventParser
{
    public string TargetUsername { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        TargetUsername = reader.ReadString();
    }
}