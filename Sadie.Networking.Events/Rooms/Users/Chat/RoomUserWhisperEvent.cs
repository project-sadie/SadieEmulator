using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users.Chat;

namespace Sadie.Networking.Events.Rooms.Users.Chat;

public class RoomUserWhisperEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomUserWhisperEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        var whisperData = reader.ReadString().Split(" ");
        var whisperUsername = whisperData.First();
        var whisperMessage = string.Join("", whisperData.Skip(1));

        if (!room.UserRepository.TryGetByUsername(whisperUsername, out var targetUser))
        {
            return;
        }

        if (!roomUser.TryCreateChatMessage(whisperMessage, (RoomChatBubble) reader.ReadInt(), out var chat))
        {
            return;
        }

        var packetBytes = new RoomUserWhisperWriter(chat).GetAllBytes();
        
        await roomUser.NetworkObject.WriteToStreamAsync(packetBytes);
        await targetUser.NetworkObject.WriteToStreamAsync(packetBytes);
    }
}