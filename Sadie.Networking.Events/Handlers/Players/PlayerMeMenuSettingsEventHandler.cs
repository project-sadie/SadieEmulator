using Sadie.Networking.Client;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerMeMenuSettings)]
public class PlayerMeMenuSettingsEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var player = client.Player!;
        var playerGameSettings = player.GameSettings;
        
        await client.WriteToStreamAsync(new PlayerMeMenuSettingsWriter
        {
            SystemVolume = playerGameSettings.SystemVolume,
            FurnitureVolume = playerGameSettings.FurnitureVolume,
            TraxVolume = playerGameSettings.TraxVolume,
            OldChat = playerGameSettings.PreferOldChat,
            BlockRoomInvites = playerGameSettings.BlockRoomInvites,
            BlockCameraFollow = playerGameSettings.BlockCameraFollow,
            UiFlags = playerGameSettings.UiFlags,
            ChatBubble = (int) player.AvatarData.ChatBubbleId
        });
    }
}