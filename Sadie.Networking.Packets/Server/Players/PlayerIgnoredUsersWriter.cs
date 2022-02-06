namespace Sadie.Networking.Packets.Server.Players;

internal class PlayerIgnoredUsersWriter : NetworkPacketWriter
{
    internal PlayerIgnoredUsersWriter() : base(ServerPacketId.PlayerIgnoredUsers)
    {
        WriteInt(0);
    }
}