using Sadie.Database.Models;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;
using Sadie.Shared.Extensions;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms;

public class RoomSettingsSaveEventHandler(
    RoomSettingsSaveEventParser eventParser, 
    IRoomRepository roomRepository, 
    RoomConstants roomConstants) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomSettingsSave;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);
        
        var (roomFound, room) = roomRepository.TryGetRoomById(eventParser.RoomId);

        if (!roomFound || room == null)
        {
            return;
        }

        if (room.OwnerId != client.Player!.Data.Id)
        {
            return;
        }
        
        if (eventParser.Tags.Any(x => x.Length > roomConstants.MaxTagLength))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter(room.Id, RoomSettingsError.TagTooLong).GetAllBytes());
            return;
        }
        
        if (string.IsNullOrEmpty(eventParser.Name))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter(room.Id, RoomSettingsError.NameRequired).GetAllBytes());
            return;
        }

        if (eventParser.AccessType == RoomAccessType.Password && string.IsNullOrEmpty(eventParser.Password))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter(room.Id, RoomSettingsError.PasswordRequired).GetAllBytes());
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
        
        var settings = room.Settings;
        
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
        settings.ChatType = eventParser.ChatType;
        settings.ChatWeight = eventParser.ChatWeight;
        settings.ChatSpeed = eventParser.ChatSpeed;
        settings.ChatDistance = eventParser.ChatDistance;
        settings.ChatProtection = eventParser.ChatProtection;
        
        await roomRepository.SaveRoomAsync(room);

        var floorSettingsWriter = new RoomWallFloorSettingsWriter(
                settings.HideWalls, 
                settings.WallThickness, 
                settings.FloorThickness).GetAllBytes();

        var settingsWriter = new RoomChatSettingsWriter(
            settings.ChatType, 
            settings.ChatWeight, 
            settings.ChatSpeed,
            settings.ChatDistance, 
            settings.ChatProtection).GetAllBytes();

        var settingsUpdatedWriter = new RoomSettingsUpdatedWriter(eventParser.RoomId).GetAllBytes();

        await room.UserRepository.BroadcastDataAsync(floorSettingsWriter);
        await room.UserRepository.BroadcastDataAsync(settingsWriter);
        await room.UserRepository.BroadcastDataAsync(settingsUpdatedWriter);
        
        await client.WriteToStreamAsync(new RoomSettingsSavedWriter(eventParser.RoomId).GetAllBytes());
    }
}