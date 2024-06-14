using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Game;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerIds.RoomUserChat)]
public class RoomUserChatEventHandler(
    RoomRepository roomRepository, 
    ServerRoomConstants roomConstants, 
    IRoomChatCommandRepository commandRepository,
    SadieContext dbContext)
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
            dbContext,
            (ChatBubble) Bubble);
    }
}