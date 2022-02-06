namespace Sadie.Networking.Packets.Server.Players.Other;

internal class PlayerIdentityWriter : NetworkPacketWriter
{
    internal PlayerIdentityWriter() : base(ServerPacketId.PlayerIdentity)
    {
        WriteInt(1);
    }
}