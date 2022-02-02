namespace Sadie.Networking.Packets.Server.Players.Effects;

public class PlayerEffectListWriter : NetworkPacketWriter
{
    public PlayerEffectListWriter() : base(ServerPacketId.PlayerEffectList)
    {
        // TODO: Pass structure in 
        
        WriteInt(0);
    }
}