using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Rooms;
using Sadie.Enums.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerId.RoomSettingsSave)]
public class RoomSettingsSaveEventHandler(
    SadieContext dbContext,
    IRoomRepository roomRepository, 
    ServerRoomConstants roomConstants) : INetworkPacketEventHandler
{
    public int RoomId { get; init; }
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

        if (room.OwnerId != client.Player!.Id)
        {
            return;
        }
        
        if (Tags.Any(x => x.Length > roomConstants.MaxTagLength))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter
            {
                RoomId = room.Id,
                ErrorCode = (int)RoomSettingsError.TagTooLong,
                Message = ""
            });
            return;
        }
        
        if (string.IsNullOrEmpty(Name))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter
            {
                RoomId = room.Id,
                ErrorCode = (int)RoomSettingsError.NameRequired,
                Message = ""
            });
            return;
        }

        if (AccessType == (int) RoomAccessType.Password && string.IsNullOrEmpty(Password))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter
            {
                RoomId = room.Id,
                ErrorCode = (int) RoomSettingsError.PasswordRequired,
                Message = ""
            });
            
            return;
        }

        room.Name = Name.Truncate(roomConstants.MaxNameLength);
        room.Description = Description.Truncate(roomConstants.MaxDescriptionLength);
        room.MaxUsersAllowed = MaxUsers;

        foreach (var tag in Tags)
        {
            room.Tags.Add(new RoomTag
            {
                Name = tag
            });
        }
        
        UpdateSettings(room.Settings);
        UpdateChatSettings(room.ChatSettings);
        
        dbContext.Entry(room).State = EntityState.Modified;
        dbContext.Entry(room.Settings).State = EntityState.Modified;
        dbContext.Entry(room.ChatSettings).State = EntityState.Modified;
        
        await dbContext.SaveChangesAsync();
        await BroadcastUpdatesAsync(room);
        
        await client.WriteToStreamAsync(new RoomSettingsSavedWriter
        {
            RoomId = RoomId
        });

        client.Player.Rooms = await dbContext
            .Rooms
            .Include(x => x.Owner)
            .Include(x => x.Settings)
            .Where(x => x.OwnerId == client.Player.Id)
            .ToListAsync();
    }

    private void UpdateSettings(RoomSettings settings)
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

    private void UpdateChatSettings(RoomChatSettings chatSettings)
    {
        chatSettings.ChatType = ChatType;
        chatSettings.ChatWeight = ChatWeight;
        chatSettings.ChatSpeed = ChatSpeed;
        chatSettings.ChatDistance = ChatDistance;
        chatSettings.ChatProtection = ChatProtection;
    }
    private async Task BroadcastUpdatesAsync(IRoomLogic room)
    {
        var settings = room.Settings;
        var chatSettings = room.ChatSettings;
        
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

        await room.UserRepository.BroadcastDataAsync(floorSettingsWriter);
        await room.UserRepository.BroadcastDataAsync(settingsWriter);
        await room.UserRepository.BroadcastDataAsync(settingsUpdatedWriter);
    }
}