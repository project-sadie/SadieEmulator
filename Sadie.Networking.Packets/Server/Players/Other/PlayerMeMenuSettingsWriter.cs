namespace Sadie.Networking.Packets.Server.Players.Other;

internal class PlayerMeMenuSettingsWriter : NetworkPacketWriter
{
    internal PlayerMeMenuSettingsWriter(int systemVolume, int furnitureVolume, int traxVolume, bool oldChat, bool blockRoomInvites, bool blockCameraFollow, int uiFlags, int chatColor) : base(ServerPacketId.PlayerMeMenuSettings)
    {
        WriteInt(systemVolume);
        WriteInt(furnitureVolume);
        WriteInt(traxVolume);
        WriteBoolean(oldChat);
        WriteBoolean(blockRoomInvites);
        WriteBoolean(blockCameraFollow);
        WriteInt(uiFlags);
        WriteInt(chatColor);
    }
}