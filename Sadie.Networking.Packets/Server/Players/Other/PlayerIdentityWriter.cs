namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerIdentityWriter : NetworkPacketWriter
{
    public PlayerIdentityWriter() : base(ServerPacketId.PlayerIdentity)
    {
        WriteInt(1);
    }
}