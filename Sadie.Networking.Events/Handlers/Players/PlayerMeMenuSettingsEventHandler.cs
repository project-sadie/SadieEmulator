using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerMeMenuSettingsEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerMeMenuSettings;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player!;
        var playerSettings = player.GameSettings;
        
        await client.WriteToStreamAsync(new PlayerMeMenuSettingsWriter
        {
            SystemVolume = playerSettings.SystemVolume,
            FurnitureVolume = playerSettings.FurnitureVolume,
            TraxVolume = playerSettings.TraxVolume,
            OldChat = playerSettings.PreferOldChat,
            BlockRoomInvites = playerSettings.BlockRoomInvites,
            BlockCameraFollow = playerSettings.BlockCameraFollow,
            UiFlags = playerSettings.UiFlags,
            ChatBubble = (int) player.AvatarData.ChatBubbleId
        });
    }
}