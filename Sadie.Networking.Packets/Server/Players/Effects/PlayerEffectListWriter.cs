namespace Sadie.Networking.Packets.Server.Players.Effects;

internal class PlayerEffectListWriter : NetworkPacketWriter
{
    internal PlayerEffectListWriter() : base(ServerPacketId.PlayerEffectList)
    {
        WriteInt(0);
    }
}