using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users.Chat;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

public class RoomUserChatEventHandler(
    RoomUserChatEventParser parser,
    IRoomRepository roomRepository, 
    RoomConstants roomConstants, 
    IRoomChatCommandRepository commandRepository)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomUserChat;

    public async Task HandleAsync(
        INetworkClient client, 
        INetworkPacketReader reader)
    {
        parser.Parse(reader);

        await NetworkPacketEventHelpers.ProcessChatMessageAsync(client,
            parser,
            false,
            roomConstants,
            roomRepository,
            commandRepository);
    }
}