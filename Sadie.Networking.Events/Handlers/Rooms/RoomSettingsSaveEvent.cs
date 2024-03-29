using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Handlers.Rooms;

public class RoomSettingsSaveEvent(RoomSettingsSaveParser parser, IRoomRepository roomRepository, RoomConstants roomConstants) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);
        
        var (roomFound, room) = roomRepository.TryGetRoomById(parser.RoomId);

        if (!roomFound || room == null)
        {
            return;
        }

        if (room.OwnerId != client.Player!.Data.Id)
        {
            return;
        }
        
        var newTags = new List<string>();
        
        for (var i = 0; i < parser.TagsCount; i++)
        {
            var label = reader.ReadString();

            if (string.IsNullOrEmpty(label) || label.Length > 20)
            {
                await client.WriteToStreamAsync(new RoomSettingsErrorWriter(room.Id, RoomSettingsError.TagTooLong).GetAllBytes());
                return;
            }
            
            newTags.Add(label);
        }
        
        if (string.IsNullOrEmpty(parser.Name))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter(room.Id, RoomSettingsError.NameRequired).GetAllBytes());
            return;
        }
        
        if (parser.Name.Length > roomConstants.MaxNameLength)
        {
            parser.Name = parser.Name.Truncate(roomConstants.MaxNameLength);
        }

        if (parser.Description.Length > roomConstants.MaxDescriptionLength)
        {
            parser.Description = parser.Description.Truncate(roomConstants.MaxDescriptionLength);
        }

        if (parser.AccessType == RoomAccessType.Password && string.IsNullOrEmpty(parser.Password))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter(room.Id, RoomSettingsError.PasswordRequired).GetAllBytes());
            return;
        }
        
        room.Name = parser.Name;
        room.Description = parser.Description;
        room.MaxUsers = parser.MaxUsers;
        room.Tags = newTags;
        
        var settings = room.Settings;
        
        settings.AccessType = parser.AccessType;
        settings.Password = parser.Password;
        settings.TradeOption = parser.TradeOption;
        settings.AllowPets = parser.AllowPets;
        settings.CanPetsEat = parser.CanPetsEat;
        settings.CanUsersOverlap = parser.CanUsersOverlap;
        settings.HideWalls = parser.HideWall;
        settings.WallThickness = parser.WallSize;
        settings.FloorThickness = parser.FloorSize;
        settings.WhoCanMute = parser.WhoCanMute;
        settings.WhoCanKick = parser.WhoCanKick;
        settings.WhoCanBan = parser.WhoCanBan;
        settings.ChatType = parser.ChatType;
        settings.ChatWeight = parser.ChatWeight;
        settings.ChatSpeed = parser.ChatSpeed;
        settings.ChatDistance = parser.ChatDistance;
        settings.ChatProtection = parser.ChatProtection;
        
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

        var settingsUpdatedWriter = new RoomSettingsUpdatedWriter(parser.RoomId).GetAllBytes();

        await room.UserRepository.BroadcastDataAsync(floorSettingsWriter);
        await room.UserRepository.BroadcastDataAsync(settingsWriter);
        await room.UserRepository.BroadcastDataAsync(settingsUpdatedWriter);
        
        await client.WriteToStreamAsync(new RoomSettingsSavedWriter(parser.RoomId).GetAllBytes());
    }
}