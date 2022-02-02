namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerMeMenuSettings : NetworkPacketWriter
{
    public PlayerMeMenuSettings() : base(ServerPacketId.PlayerMeMenuSettings)
    {
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