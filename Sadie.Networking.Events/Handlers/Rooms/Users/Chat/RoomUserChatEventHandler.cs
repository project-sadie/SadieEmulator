using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users.Chat;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerIds.RoomUserChat)]
public class RoomUserChatEventHandler(
    RoomUserChatEventParser parser,
    RoomRepository roomRepository, 
    ServerRoomConstants roomConstants, 
    IRoomChatCommandRepository commandRepository,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(
        INetworkClient client, 
        INetworkPacketReader reader)
    {
        parser.Parse(reader);

        await NetworkPacketEventHelpers.OnChatMessageAsync(client,
            parser,
            false,
            roomConstants,
            roomRepository,
            commandRepository,
            dbContext);
    }
}