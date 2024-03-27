using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;
using Sadie.Shared.Unsorted.Game;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerMeMenuSettingsWriter : NetworkPacketWriter
{
    public PlayerMeMenuSettingsWriter(
        int systemVolume, 
        int furnitureVolume, 
        int traxVolume, 
        bool oldChat, 
        bool blockRoomInvites, 
        bool blockCameraFollow, 
        int uiFlags, 
        ChatBubble chatBubble)
    {
        WriteShort(ServerPacketId.PlayerMeMenuSettings);
        WriteInteger(systemVolume);
        WriteInteger(furnitureVolume);
        WriteInteger(traxVolume);
        WriteBool(oldChat);
        WriteBool(blockRoomInvites);
        WriteBool(blockCameraFollow);
        WriteInteger(uiFlags);
        WriteInteger((int) chatBubble);
    }
}