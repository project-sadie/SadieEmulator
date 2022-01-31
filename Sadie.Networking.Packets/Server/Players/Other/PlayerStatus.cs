namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerStatus : NetworkPacketWriter
{
    public PlayerStatus() : base(ServerPacketIds.PlayerStatus)
    {
        WriteBoolean(true);
        WriteBoolean(false);
        WriteBoolean(true);
    }
}