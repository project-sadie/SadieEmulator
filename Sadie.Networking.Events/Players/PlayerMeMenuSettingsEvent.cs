using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Players;

public class PlayerMeMenuSettingsEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var settings = client.Player.Settings;
        
        await client.WriteToStreamAsync(new PlayerMeMenuSettingsWriter(
            settings.SystemVolume, 
            settings.FurnitureVolume, 
            settings.TraxVolume, 
            settings.OldChat, 
            settings.BlockRoomInvites, 
            settings.BlockCameraFollow, 
            settings.UiFlags, 
            settings.ChatColor).GetAllBytes());
    }
}