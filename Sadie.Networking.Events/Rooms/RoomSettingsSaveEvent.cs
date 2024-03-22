using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Rooms;

public class RoomSettingsSaveEvent(IRoomRepository roomRepository, RoomConstants roomConstants) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var roomId = reader.ReadInteger();
        var (roomFound, room) = roomRepository.TryGetRoomById(roomId);

        if (!roomFound || room == null)
        {
            return;
        }

        if (room.OwnerId != client.Player!.Data.Id)
        {
            return;
        }

        var newName = reader.ReadString();
        var newDescription = reader.ReadString();
        var newAccessType = (RoomAccessType) reader.ReadInteger();
        var newPassword = reader.ReadString();
        var newMaxUsers = reader.ReadInteger();
        var newCategoryId = reader.ReadInteger();
        var newTagsCount = reader.ReadInteger();
        
        var newTags = new List<string>();
        
        for (var i = 0; i < newTagsCount; i++)
        {
            var label = reader.ReadString();

            if (string.IsNullOrEmpty(label) || label.Length > 20)
            {
                await client.WriteToStreamAsync(new RoomSettingsErrorWriter(room.Id, RoomSettingsError.TagTooLong).GetAllBytes());
                return;
            }
            
            newTags.Add(label);
        }
        
        if (string.IsNullOrEmpty(newName))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter(room.Id, RoomSettingsError.NameRequired).GetAllBytes());
            return;
        }
        
        if (newName.Length > roomConstants.MaxNameLength)
        {
            newName = newName.Truncate(roomConstants.MaxNameLength);
        }

        if (newDescription.Length > roomConstants.MaxDescriptionLength)
        {
            newDescription = newDescription.Truncate(roomConstants.MaxDescriptionLength);
        }

        if (newAccessType == RoomAccessType.Password && string.IsNullOrEmpty(newPassword))
        {
            await client.WriteToStreamAsync(new RoomSettingsErrorWriter(room.Id, RoomSettingsError.PasswordRequired).GetAllBytes());
            return;
        }
        
        var newTradeOption = reader.ReadInteger();
        var newAllowPets = reader.ReadBool();
        var newCanPetsEat = reader.ReadBool();
        var newCanUsersOverlap = reader.ReadBool();
        var newHideWall = reader.ReadBool();
        var newWallSize = reader.ReadInteger();
        var newFloorSize = reader.ReadInteger();
        var newWhoCanMute = reader.ReadInteger();
        var newWhoCanKick = reader.ReadInteger();
        var newWhoCanBan = reader.ReadInteger();
        var newChatType = reader.ReadInteger();
        var newChatWeight = reader.ReadInteger();
        var newChatSpeed = reader.ReadInteger();
        var newChatDistance = reader.ReadInteger();
        var newChatProtection = reader.ReadInteger();
        
        room.Name = newName;
        room.Description = newDescription;
        room.MaxUsers = newMaxUsers;
        room.Tags = newTags;
        
        var settings = room.Settings;
        
        settings.AccessType = newAccessType;
        settings.Password = newPassword;
        settings.TradeOption = newTradeOption;
        settings.AllowPets = newAllowPets;
        settings.CanPetsEat = newCanPetsEat;
        settings.CanUsersOverlap = newCanUsersOverlap;
        settings.HideWalls = newHideWall;
        settings.WallThickness = newWallSize;
        settings.FloorThickness = newFloorSize;
        settings.WhoCanMute = newWhoCanMute;
        settings.WhoCanKick = newWhoCanKick;
        settings.WhoCanBan = newWhoCanBan;
        settings.ChatType = newChatType;
        settings.ChatWeight = newChatWeight;
        settings.ChatSpeed = newChatSpeed;
        settings.ChatDistance = newChatDistance;
        settings.ChatProtection = newChatProtection;
        
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

        var settingsUpdatedWriter = new RoomSettingsUpdatedWriter(roomId).GetAllBytes();

        await room.UserRepository.BroadcastDataAsync(floorSettingsWriter);
        await room.UserRepository.BroadcastDataAsync(settingsWriter);
        await room.UserRepository.BroadcastDataAsync(settingsUpdatedWriter);
        
        await client.WriteToStreamAsync(new RoomSettingsSavedWriter(roomId).GetAllBytes());
    }
}