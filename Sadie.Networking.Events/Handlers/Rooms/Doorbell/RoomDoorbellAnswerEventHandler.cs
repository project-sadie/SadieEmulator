using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Doorbell;

namespace Sadie.Networking.Events.Handlers.Rooms.Doorbell;

[PacketId(EventHandlerIds.RoomDoorbellAnswer)]
public class RoomDoorbellAnswerEventHandler(
    PlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public required string Username { get; set; }
    public bool Accept { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var username = Username;
        var player = playerRepository.GetPlayerLogicByUsername(username);
        
        if (player == null)
        {
            return;
        }

        await player.NetworkObject.WriteToStreamAsync(Accept
            ? new RoomDoorbellAcceptWriter { Username = username }
            : new RoomDoorbellNoAnswerWriter { Username = username });
    }
}