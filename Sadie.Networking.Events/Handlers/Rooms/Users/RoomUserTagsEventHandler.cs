using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerIds.RoomUserTags)]
public class RoomUserTagsEventHandler(RoomUserTagsEventParser eventParser, RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);
        
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
        {
            return;
        }

        if (room.UserRepository.TryGet(eventParser.UserId, out var specialUser))
        {        
            await specialUser!.NetworkObject.WriteToStreamAsync(new RoomUserTagsWriter
            {
                UserId = specialUser.Id,
                Tags = specialUser.Player.Tags.Select(x => x.Name).ToList()
            });
        }
    }
}