using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserTags)]
public class RoomUserTagsEventHandler(RoomRepository roomRepository, SadieContext dbContext) : INetworkPacketEventHandler
{
    public int UserId { get; set; }

    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
        {
            return;
        }

        if (room.UserRepository.TryGetById(UserId, out var user))
        {
            if (user!.Player.Tags == null)
            {
                user.Player.Tags = await dbContext
                    .PlayerTags
                    .Where(x => x.PlayerId == user.Id)
                    .ToListAsync();
            }
            
            await user.NetworkObject.WriteToStreamAsync(new RoomUserTagsWriter
            {
                UserId = user.Id,
                Tags = user.Player.Tags.Select(x => x.Name ?? "")
            });
        }
    }
}