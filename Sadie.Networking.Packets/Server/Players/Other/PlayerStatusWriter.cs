namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerStatusWriter : NetworkPacketWriter
{
    public PlayerStatusWriter() : base(ServerPacketId.PlayerStatus)
    {
        WriteBoolean(true);
        WriteBoolean(false);
        WriteBoolean(true);
    }
}