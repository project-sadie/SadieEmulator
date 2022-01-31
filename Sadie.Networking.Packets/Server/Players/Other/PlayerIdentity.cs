namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerIdentity : NetworkPacketWriter
{
    public PlayerIdentity() : base(ServerPacketIds.PlayerIdentity)
    {
        WriteInt(1);
    }
}