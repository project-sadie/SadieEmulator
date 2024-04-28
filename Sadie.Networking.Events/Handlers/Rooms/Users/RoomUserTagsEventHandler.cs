using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserTagsEventHandler(RoomUserTagsEventParser eventParser, RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomUserTags;

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
                Tags = specialUser.Player.Tags
            });
        }
    }
}