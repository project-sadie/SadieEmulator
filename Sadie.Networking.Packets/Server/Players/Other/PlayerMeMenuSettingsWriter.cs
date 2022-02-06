namespace Sadie.Networking.Packets.Server.Players.Other;

internal class PlayerMeMenuSettingsWriter : NetworkPacketWriter
{
    internal PlayerMeMenuSettingsWriter() : base(ServerPacketId.PlayerMeMenuSettings)
    {
        WriteInt(100);
        WriteInt(100);
        WriteInt(100); 
        WriteBoolean(false);
        WriteBoolean(false);
        WriteBoolean(false);
        WriteInt(0);
        WriteInt(1);
    }
}