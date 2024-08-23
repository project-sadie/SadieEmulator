using Sadie.API.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserTags)]
public class RoomUserTagsEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int UserId { get; set; }

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
                UserId = specialUser.Id,
                Tags = specialUser.Player.Tags.Select(x => x.Name).ToList()
            });
        }
    }
}