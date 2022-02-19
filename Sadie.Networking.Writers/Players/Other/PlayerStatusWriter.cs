namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerStatusWriter : NetworkPacketWriter
{
    public PlayerStatusWriter(bool isOpen, bool isShuttingDown, bool isAuthentic) : base(ServerPacketId.PlayerStatus)
    {
        WriteBoolean(isOpen);
        WriteBoolean(isShuttingDown);
        WriteBoolean(isAuthentic);
    }
}