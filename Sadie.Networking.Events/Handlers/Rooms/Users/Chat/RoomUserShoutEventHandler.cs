using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users.Chat;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

public class RoomUserShoutEventHandler(
    RoomUserChatEventParser eventParser,
    IRoomRepository roomRepository, 
    RoomConstants roomConstants, 
    IRoomChatCommandRepository commandRepository)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var message = eventParser.Message;
        
        if (string.IsNullOrEmpty(message) || message.Length > roomConstants.MaxChatMessageLength)
        {
            return;
        }
        
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
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
            eventParser.Bubble, 
            0, 
            RoomChatMessageType.Shout);
        
        await roomUser.OnTalkAsync(chatMessage);
        await room!.UserRepository.BroadcastDataAsync(new RoomUserShoutWriter(chatMessage!, 0).GetAllBytes());
    }
}