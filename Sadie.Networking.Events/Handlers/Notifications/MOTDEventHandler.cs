using Sadie.Database.Models;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Shared;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Notifications;

public class MOTDEventHandler(ServerSettings serverSettings) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.MOTD;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (string.IsNullOrEmpty(serverSettings.PlayerWelcomeMessage))
        {
            return;
        }

        var formattedMessage = serverSettings.PlayerWelcomeMessage
            .Replace("[username]", client.Player.Username)
            .Replace("[version]", GlobalState.Version.ToString());

        await client.Player.NetworkObject.WriteToStreamAsync(new PlayerAlertWriter(formattedMessage).GetAllBytes());
    }
}
