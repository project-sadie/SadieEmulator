using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Chat.Commands;
using Sadie.API.Interfaces.Game.Rooms.Services;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Core.Shared.Attributes;
using Sadie.Db.Models.Constants;
using Sadie.Networking.Events.Application;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerId.RoomUserShout)]
public class RoomUserShoutEventHandler(
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
        await RoomChatService.OnChatMessageAsync(client,
            Message,
            true,
            roomConstants,
            roomRepository,
            commandRepository,
            (ChatBubble) Bubble,
            wiredService,
            roomHelperService);
    }
}