namespace Sadie.Networking.Packets.Server.Players.Effects;

public class PlayerEffectList : NetworkPacketWriter
{
    public PlayerEffectList() : base(ServerPacketIds.PlayerEffectList)
    {
        WriteInt(0);
    }
}