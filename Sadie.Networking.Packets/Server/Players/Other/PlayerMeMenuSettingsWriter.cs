namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerMeMenuSettingsWriter : NetworkPacketWriter
{
    public PlayerMeMenuSettingsWriter() : base(ServerPacketId.PlayerMeMenuSettings)
    {
        // TODO: Pass structure in 
        
        WriteInt(100);
        WriteInt(100);
        WriteInt(100); 
        WriteBoolean(false);
        WriteBoolean(false);
        WriteBoolean(false);
        WriteInt(0); // ?
        WriteInt(1);
    }
}