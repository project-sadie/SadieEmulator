using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Packets.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserTags)]
public class RoomUserTagsEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int UserId { get; init; }

    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
        {
            return;
        }

        if (room.UserRepository.TryGetById(UserId, out var specialUser))
        {        
            await specialUser!.NetworkObject.WriteToStreamAsync(new RoomUserTagsWriter
            {
                UserId = specialUser.Player.Player.Id,
                Tags = specialUser.Player.Player.Tags.Select(x => x.Name).ToList()
            });
        }
    }
}