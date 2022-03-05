using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerMeMenuSettingsWriter : NetworkPacketWriter
{
    public PlayerMeMenuSettingsWriter(int systemVolume, int furnitureVolume, int traxVolume, bool oldChat, bool blockRoomInvites, bool blockCameraFollow, int uiFlags, int chatColor) : base(ServerPacketId.PlayerMeMenuSettings)
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