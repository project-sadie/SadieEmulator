using Sadie.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerIgnoredUsersWriter : NetworkPacketWriter
{
    public PlayerIgnoredUsersWriter() : base(ServerPacketId.PlayerIgnoredUsers)
    {
        WriteInt(0);
    }
}