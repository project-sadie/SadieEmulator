using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Enums;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerIds.RoomSettingsSave)]
public class RoomSettingsSaveEventHandler(
    RoomRepository roomRepository, 
    ServerRoomConstants roomConstants) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);
        
        var room = roomRepository.TryGetRoomById(eventParser.RoomId);

        if (room == null)
        {
            return;
        }

        if (room.OwnerId != client.Player!.Id)
        {
            return;
        }
        
        if (eventParser.Tags.Any(x => x.Length > roomConstants.MaxTagLength))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter
            {
                RoomId = room.Id,
                ErrorCode = (int)RoomSettingsError.TagTooLong,
                Unknown = ""
            });
            return;
        }
        
        if (string.IsNullOrEmpty(eventParser.Name))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter
            {
                RoomId = room.Id,
                ErrorCode = (int)RoomSettingsError.NameRequired,
                Unknown = ""
            });
            return;
        }

        if (eventParser.AccessType == RoomAccessType.Password && string.IsNullOrEmpty(eventParser.Password))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter
            {
                RoomId = room.Id,
                ErrorCode = (int) RoomSettingsError.PasswordRequired,
                Unknown = ""
            });
            return;
        }

        room.Name = eventParser.Name.Truncate(roomConstants.MaxNameLength);
        room.Description = eventParser.Description.Truncate(roomConstants.MaxDescriptionLength);
        room.MaxUsersAllowed = eventParser.MaxUsers;

        foreach (var tag in eventParser.Tags)
        {
            room.Tags.Add(new RoomTag
            {
                Name = tag
            });
        }
        
        UpdateSettings(room.Settings);
        UpdateChatSettings(room.ChatSettings);
        
        await roomRepository.SaveRoomAsync(room);
        await BroadcastUpdatesAsync(room);
        
        await client.WriteToStreamAsync(new RoomSettingsSavedWriter
        {
            RoomId = eventParser.RoomId
        });
    }

    private void UpdateSettings(RoomSettings settings)
    {
        settings.AccessType = eventParser.AccessType;
        settings.Password = eventParser.Password;
        settings.TradeOption = eventParser.TradeOption;
        settings.AllowPets = eventParser.AllowPets;
        settings.CanPetsEat = eventParser.CanPetsEat;
        settings.CanUsersOverlap = eventParser.CanUsersOverlap;
        settings.HideWalls = eventParser.HideWall;
        settings.WallThickness = eventParser.WallSize;
        settings.FloorThickness = eventParser.FloorSize;
        settings.WhoCanMute = eventParser.WhoCanMute;
        settings.WhoCanKick = eventParser.WhoCanKick;
        settings.WhoCanBan = eventParser.WhoCanBan;
    }

    private void UpdateChatSettings(RoomChatSettings chatSettings)
    {
        chatSettings.ChatType = eventParser.ChatType;
        chatSettings.ChatWeight = eventParser.ChatWeight;
        chatSettings.ChatSpeed = eventParser.ChatSpeed;
        chatSettings.ChatDistance = eventParser.ChatDistance;
        chatSettings.ChatProtection = eventParser.ChatProtection;
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
            RoomId = eventParser.RoomId
        };

        await room.UserRepository.BroadcastDataAsync(floorSettingsWriter);
        await room.UserRepository.BroadcastDataAsync(settingsWriter);
        await room.UserRepository.BroadcastDataAsync(settingsUpdatedWriter);
    }
}