using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users.Chat;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Extensions;
using Sadie.Shared.Unsorted.Game;

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

        var message = parser.Message;

        if (message.Length > roomConstants.MaxChatMessageLength)
        {
            message = message.Truncate(roomConstants.MaxChatMessageLength - 1) + "...";
        }
        
        if (string.IsNullOrEmpty(message))
        {
            return;
        }
        
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        if (message.StartsWith(":"))
        {
            var (found, command) = commandRepository.TryGetCommandByTriggerWord(message.Split(" ")[0].Substring(1));

            if (found && command != null)
            {
                await command.ExecuteAsync(roomUser!);
                return;
            }
        }
        
        var chatMessage = new RoomChatMessage(
            roomUser, 
            message, 
            room, 
            parser.Bubble, 
            0, 
            RoomChatMessageType.Normal);
        
        await roomUser.OnTalkAsync(chatMessage);
        await room!.UserRepository.BroadcastDataAsync(new RoomUserChatWriter(chatMessage, 0).GetAllBytes());
    }
}