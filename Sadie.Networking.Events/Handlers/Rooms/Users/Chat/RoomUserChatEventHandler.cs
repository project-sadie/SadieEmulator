using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Services;
using Sadie.Db.Models.Constants;
using Sadie.Enums.Miscellaneous;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerId.RoomUserChat)]
public class RoomUserChatEventHandler(
    IRoomRepository roomRepository, 
    ServerRoomConstants roomConstants, 
    IRoomChatCommandRepository commandRepository,
    IRoomWiredService wiredService,
    IRoomHelperService roomHelperService)
    : INetworkPacketEventHandler
{
    public required string Message { get; init; }
    public int Bubble { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        await NetworkPacketEventHelpers.OnChatMessageAsync(client,
            Message,
            false,
            roomConstants,
            roomRepository,
            commandRepository,
            (ChatBubble) Bubble,
            wiredService,
            roomHelperService);
    }
}