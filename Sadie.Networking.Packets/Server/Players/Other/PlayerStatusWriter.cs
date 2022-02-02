namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerStatusWriter : NetworkPacketWriter
{
    public PlayerStatusWriter() : base(ServerPacketId.PlayerStatus)
    {
        // TODO: Pass structure in 
        
        WriteBoolean(true);
        WriteBoolean(false);
        WriteBoolean(true);
    }
}