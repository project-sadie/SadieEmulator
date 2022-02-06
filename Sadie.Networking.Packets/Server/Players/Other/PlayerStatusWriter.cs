namespace Sadie.Networking.Packets.Server.Players.Other;

internal class PlayerStatusWriter : NetworkPacketWriter
{
    internal PlayerStatusWriter(bool isOpen, bool isShuttingDown, bool isAuthentic) : base(ServerPacketId.PlayerStatus)
    {
        WriteBoolean(isOpen);
        WriteBoolean(isShuttingDown);
        WriteBoolean(isAuthentic);
    }
}