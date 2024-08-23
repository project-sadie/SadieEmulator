using Sadie.Database.Models.Constants;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Game.Rooms.Services;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerId.RoomUserShout)]
public class RoomUserShoutEventHandler(
    RoomRepository roomRepository, 
    ServerRoomConstants roomConstants, 
    IRoomChatCommandRepository commandRepository,
    IRoomWiredService wiredService)
    : INetworkPacketEventHandler
{
    public required string Message { get; set; }
    public int Bubble { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        await NetworkPacketEventHelpers.OnChatMessageAsync(client,
            Message,
            true,
            roomConstants,
            roomRepository,
            commandRepository,
            (ChatBubble) Bubble,
            wiredService);
    }
}