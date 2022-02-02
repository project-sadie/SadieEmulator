namespace Sadie.Networking.Packets.Server.Players.Effects;

public class PlayerEffectList : NetworkPacketWriter
{
    public PlayerEffectList() : base(ServerPacketId.PlayerEffectList)
    {
        WriteInt(0);
    }
}