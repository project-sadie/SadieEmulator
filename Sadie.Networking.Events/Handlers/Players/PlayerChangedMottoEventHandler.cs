using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerIds.PlayerChangedMotto)]
public class PlayerChangedMottoEventHandler(
    RoomRepository roomRepository, 
    ServerPlayerConstants constants,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public required string Motto { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null)
        {
            return;
        }
        
        var player = client.Player!;
        var newMotto = Motto.Truncate(constants.MaxMottoLength);

        player.AvatarData.Motto = newMotto;
        
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserDataWriter{
            Users = [roomUser]
        });
        
        dbContext.PlayerAvatarData.Update(player.AvatarData);
        await dbContext.SaveChangesAsync();
    }
}