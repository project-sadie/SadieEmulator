namespace Sadie.Networking.Packets.Server.Players.Effects;

public class PlayerEffectListWriter : NetworkPacketWriter
{
    public PlayerEffectListWriter() : base(ServerPacketId.PlayerEffectList)
    {
        WriteInt(0);
    }
}