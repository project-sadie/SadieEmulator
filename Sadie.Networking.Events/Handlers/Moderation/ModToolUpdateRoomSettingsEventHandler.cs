using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.Db;
using Sadie.Enums.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Moderation;

[PacketId(EventHandlerId.ModToolsUpdateRoomSettings)]
public class ModToolUpdateRoomSettingsEventHandler(
    IRoomRepository roomRepository,
    IDbContextFactory<SadieContext> dbContextFactory) : INetworkPacketEventHandler
{
    public int RoomId { get; set; }
    public int LockDoor { get; set; }
    public int ChangeTitle { get; set; }
    public int KickUsers { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var room = roomRepository.TryGetRoomById(RoomId);
        
        if (room == null)
        {
            return;
        }
        
        var needsSaving = false;
        
        if (LockDoor == 1)
        {
            room.Settings.AccessType = RoomAccessType.Doorbell;
            needsSaving = true;
        }

        if (ChangeTitle == 1)
        {
            room.Name = "Inappropriate to hotel management.";
            needsSaving = true;
        }

        if (KickUsers == 1)
        {
            foreach (var user in room.UserRepository.GetAll())
            {
                await room.UserRepository.TryRemoveAsync(user.Player.Id, true, true);
            }
        }

        if (needsSaving)
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync();
            dbContext.Entry(room).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }
    }
}