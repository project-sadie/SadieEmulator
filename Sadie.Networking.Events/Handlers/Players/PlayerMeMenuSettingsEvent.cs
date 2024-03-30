using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerMeMenuSettingsEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player!;
        var playerSettings = player.Data.Settings;
        
        await client.WriteToStreamAsync(new PlayerMeMenuSettingsWriter(
            playerSettings.SystemVolume, 
            playerSettings.FurnitureVolume, 
            playerSettings.TraxVolume, 
            playerSettings.OldChat, 
            playerSettings.BlockRoomInvites, 
            playerSettings.BlockCameraFollow, 
            playerSettings.UiFlags, 
            player.Data.ChatBubble).GetAllBytes());
    }
}