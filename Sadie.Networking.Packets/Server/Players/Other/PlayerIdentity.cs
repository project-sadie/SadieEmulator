namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerIdentity : NetworkPacketWriter
{
    public PlayerIdentity() : base(ServerPacketId.PlayerIdentity)
    {
        WriteInt(1);
    }
}