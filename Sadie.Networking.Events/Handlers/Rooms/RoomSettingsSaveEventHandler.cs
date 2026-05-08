using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Rooms;
using Sadie.Core.Shared.Attributes;
using Sadie.Core.Shared.Extensions;
using Sadie.Db;
using Sadie.Db.Models.Constants;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerId.RoomSettingsSave)]
public class RoomSettingsSaveEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomRepository roomRepository, 
    ServerRoomConstants roomConstants) : INetworkPacketEventHandler
{
    public long RoomId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public int AccessType { get; init; }
    public required string Password { get; init; }
    public int MaxUsers { get; init; }
    public int CategoryId { get; init; }
    public List<string> Tags { get; init; } = [];
    public int TradeOption { get; init; }
    public bool AllowPets { get; init; }
    public bool CanPetsEat { get; init; }
    public bool CanUsersOverlap { get; init; }
    public bool HideWall { get; init; }
    public int WallSize { get; init; }
    public int FloorSize { get; init; }
    public int WhoCanMute { get; init; }
    public int WhoCanKick { get; init; }
    public int WhoCanBan { get; init; }
    public int ChatType { get; init; }
    public int ChatWeight { get; init; }
    public int ChatSpeed { get; init; }
    public int ChatDistance { get; init; }
    public int ChatProtection { get; init; }

    public async Task HandleAsync(INetworkClient client)
    {
        var room = roomRepository.TryGetRoomById(RoomId);

        if (room == null)
        {
            return;
        }

        if (room.Room.OwnerId != client.Player!.Player.Id)
        {
            return;
        }
        
        if (Tags.Any(x => x.Length > roomConstants.MaxTagLength))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter
            {
                RoomId = room.Room.Id,
                ErrorCode = (int)RoomSettingsError.TagTooLong,
                Message = ""
            });
            return;
        }
        
        if (string.IsNullOrEmpty(Name))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter
            {
                RoomId = room.Room.Id,
                ErrorCode = (int)RoomSettingsError.NameRequired,
                Message = ""
            });
            return;
        }

        if (AccessType == (int) RoomAccessType.Password && string.IsNullOrEmpty(Password))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter
            {
                RoomId = room.Room.Id,
                ErrorCode = (int) RoomSettingsError.PasswordRequired,
                Message = ""
            });
            
            return;
        }

        room.Room.Name = Name.Truncate(roomConstants.MaxNameLength);
        room.Room.Description = Description.Truncate(roomConstants.MaxDescriptionLength);
        room.Room.MaxUsersAllowed = MaxUsers;

        foreach (var tag in Tags)
        {
            room.Room.Tags.Add(new RoomTagDto
            {
                Name = tag
            });
        }
        
        UpdateSettings(room.Room.Settings);
        UpdateChatSettings(room.Room.ChatSettings);
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(room).State = EntityState.Modified;
        dbContext.Entry(room.Room.Settings).State = EntityState.Modified;
        dbContext.Entry(room.Room.ChatSettings).State = EntityState.Modified;
        
        await dbContext.SaveChangesAsync();
        await BroadcastUpdatesAsync(room);
        
        await client.WriteToStreamAsync(new RoomSettingsSavedWriter
        {
            RoomId = RoomId
        });
    }

    private void UpdateSettings(RoomSettingsDto settings)
    {
        settings.AccessType = (RoomAccessType) AccessType;
        settings.Password = Password;
        settings.TradeOption = (RoomTradeOption) TradeOption;
        settings.AllowPets = AllowPets;
        settings.CanPetsEat = CanPetsEat;
        settings.CanUsersOverlap = CanUsersOverlap;
        settings.HideWalls = HideWall;
        settings.WallThickness = WallSize;
        settings.FloorThickness = FloorSize;
        settings.WhoCanMute = WhoCanMute;
        settings.WhoCanKick = WhoCanKick;
        settings.WhoCanBan = WhoCanBan;
    }

    private void UpdateChatSettings(RoomChatSettingsDto chatSettings)
    {
        chatSettings.ChatType = ChatType;
        chatSettings.ChatWeight = ChatWeight;
        chatSettings.ChatSpeed = ChatSpeed;
        chatSettings.ChatDistance = ChatDistance;
        chatSettings.ChatProtection = ChatProtection;
    }
    private async Task BroadcastUpdatesAsync(IRoomLogic room)
    {
        var settings = room.Room.Settings;
        var chatSettings = room.Room.ChatSettings;
        
        var floorSettingsWriter = new RoomWallFloorSettingsWriter
        {
            HideWalls = settings.HideWalls,
            WallThickness = settings.WallThickness,
            FloorThickness = settings.FloorThickness
        };

        var settingsWriter = new RoomChatSettingsWriter
        {
            ChatType = chatSettings.ChatType,
            ChatWeight = chatSettings.ChatWeight,
            ChatSpeed = chatSettings.ChatSpeed,
            ChatDistance = chatSettings.ChatDistance,
            ChatProtection = chatSettings.ChatProtection
        };

        var settingsUpdatedWriter = new RoomSettingsUpdatedWriter
        {
            RoomId = RoomId
        };

        await room.BroadcastDataAsync(floorSettingsWriter);
        await room.BroadcastDataAsync(settingsWriter);
        await room.BroadcastDataAsync(settingsUpdatedWriter);
    }
}