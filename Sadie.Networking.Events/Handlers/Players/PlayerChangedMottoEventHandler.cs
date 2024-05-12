using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerChangedMottoEventHandler(
    PlayerChangedMottoEventParser eventParser,
    RoomRepository roomRepository, 
    ServerPlayerConstants constants,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerChangedMotto;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null)
        {
            return;
        }
        
        eventParser.Parse(reader);

        var player = client.Player!;
        var newMotto = eventParser.Motto.Truncate(constants.MaxMottoLength);

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