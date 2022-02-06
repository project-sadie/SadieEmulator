namespace Sadie.Networking.Packets.Server.Players.Other;

internal class PlayerPongWriter : NetworkPacketWriter
{
    internal PlayerPongWriter(int id) : base(ServerPacketId.PlayerPong)
    {
        WriteInt(id);
    }
}