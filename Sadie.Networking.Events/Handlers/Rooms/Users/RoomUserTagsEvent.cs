using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserTagsEvent(RoomUserTagsParser parser, IRoomRepository roomRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);
        
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
        {
            return;
        }

        if (room != null && room.UserRepository.TryGet(parser.UserId, out var specialUser))
        {        
            await specialUser!.NetworkObject.WriteToStreamAsync(new RoomUserTagsWriter(specialUser.Id, specialUser.AvatarData.Tags).GetAllBytes());
        }
    }
}