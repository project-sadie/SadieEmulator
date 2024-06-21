using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Rooms;
using Sadie.Enums.Game.Rooms;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Enums;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms;
using Sadie.Shared.Extensions;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerId.RoomSettingsSave)]
public class RoomSettingsSaveEventHandler(
    SadieContext dbContext,
    RoomRepository roomRepository, 
    ServerRoomConstants roomConstants) : INetworkPacketEventHandler
{
    public long RoomId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int AccessType { get; set; }
    public required string Password { get; set; }
    public int MaxUsers { get; set; }
    public int CategoryId { get; set; }
    public List<string> Tags { get; set; } = [];
    public int TradeOption { get; set; }
    public bool AllowPets { get; set; }
    public bool CanPetsEat { get; set; }
    public bool CanUsersOverlap { get; set; }
    public bool HideWall { get; set; }
    public int WallSize { get; set; }
    public int FloorSize { get; set; }
    public int WhoCanMute { get; set; }
    public int WhoCanKick { get; set; }
    public int WhoCanBan { get; set; }
    public int ChatType { get; set; }
    public int ChatWeight { get; set; }
    public int ChatSpeed { get; set; }
    public int ChatDistance { get; set; }
    public int ChatProtection { get; set; }

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
                Unknown = ""
            });
            return;
        }
        
        if (string.IsNullOrEmpty(Name))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter
            {
                RoomId = room.Id,
                ErrorCode = (int)RoomSettingsError.NameRequired,
                Unknown = ""
            });
            return;
        }

        if (AccessType == (int) RoomAccessType.Password && string.IsNullOrEmpty(Password))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter
            {
                RoomId = room.Id,
                ErrorCode = (int) RoomSettingsError.PasswordRequired,
                Unknown = ""
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
    private async Task BroadcastUpdatesAsync(RoomLogic room)
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