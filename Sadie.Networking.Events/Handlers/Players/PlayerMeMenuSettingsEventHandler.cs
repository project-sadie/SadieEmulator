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
        
        await client.WriteToStreamAsync(new PlayerMeMenuSettingsWriter(
            playerSettings.SystemVolume, 
            playerSettings.FurnitureVolume, 
            playerSettings.TraxVolume, 
            playerSettings.PreferOldChat, 
            playerSettings.BlockRoomInvites, 
            playerSettings.BlockCameraFollow, 
            playerSettings.UiFlags, 
            player.AvatarData.ChatBubbleId).GetAllBytes());
    }
}