using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Services;
using Sadie.Database.Models.Constants;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

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
    public required string Message { get; set; }
    public int Bubble { get; set; }
    
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