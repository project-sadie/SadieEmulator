using Microsoft.Extensions.Logging;
using Sadie.Game.Players.Room;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users.Chat;
using Sadie.Networking.Writers.Generic;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Events;

internal static class NetworkPacketEventHelpers
{
    internal static bool TryResolveRoomObjectsForClient(
        IRoomRepository roomRepository, 
        INetworkClient client, out IRoom room, out IRoomUser user)
    {
        var player = client.Player;
        
        if (player == null)
        {
            room = null!;
            user = null!;
            
            return false;
        }
        
        var roomId = player.Data.CurrentRoomId;
        var (result, roomObject) = roomRepository.TryGetRoomById(roomId);

        if (!result || roomObject == null || client.RoomUser == null)
        {
            room = null!;
            user = null!;
            
            return false;
        }

        room = roomObject;
        user = client.RoomUser;
        
        return true;
    }

    internal static async Task EnterRoomAsync<T>(INetworkClient client, IRoom room, ILogger<T> logger, 
        IRoomUserFactory roomUserFactory)
    {
        var player = client.Player;
        var playerData = player.Data;
        var playerState = player.State;
        
        playerData.CurrentRoomId = room.Id;
        playerState.RoomVisits.Add(new PlayerRoomVisit(playerData.Id, room.Id));

        var avatarData = (IAvatarData) player.Data;

        var controllerLevel = RoomControllerLevel.None;
        
        if (room.PlayersWithRights.Contains(playerData.Id))
        {
            controllerLevel = RoomControllerLevel.Rights;
        }

        if (room.OwnerId == player.Data.Id)
        {
            controllerLevel = RoomControllerLevel.Owner;
        }
        
        var roomUser = roomUserFactory.Create(
            room,
            player.NetworkObject,
            playerData.Id,
            room.Layout.DoorPoint,
            room.Layout.DoorDirection,
            room.Layout.DoorDirection,
            avatarData,
            controllerLevel);

        roomUser.ApplyFlatCtrlStatus();
        
        if (!room.UserRepository.TryAdd(roomUser))
        {
            logger.LogError($"Failed to add user {playerData.Id} to room {room.Id}");
            return;
        }
        
        client.RoomUser = roomUser;

        var canLikeRoom = !player.Data.LikedRoomIds.Contains(room.Id);
        
        await client.WriteToStreamAsync(new RoomDataWriter(room.Id, room.Layout.Name).GetAllBytes());
        
        await client.WriteToStreamAsync(new RoomPaintWriter("floor", "0.0").GetAllBytes());
        await client.WriteToStreamAsync(new RoomPaintWriter("wallpaper", "0.0").GetAllBytes());
        await client.WriteToStreamAsync(new RoomPaintWriter("landscape", "0.0").GetAllBytes());
        
        await client.WriteToStreamAsync(new RoomScoreWriter(room.Score, canLikeRoom).GetAllBytes());
        await client.WriteToStreamAsync(new RoomPromotionWriter().GetAllBytes());

        var owner = room.OwnerId == playerData.Id;
        
        await client.WriteToStreamAsync(new RoomPaneWriter(room.Id, owner).GetAllBytes());
        await client.WriteToStreamAsync(new RoomRightsWriter(roomUser.ControllerLevel).GetAllBytes());
        
        if (owner)
        {
            await client.WriteToStreamAsync(new RoomOwnerWriter().GetAllBytes());
        }

        await client.WriteToStreamAsync(new RoomLoadedWriter().GetAllBytes());
    }

    public static async Task SendFurniturePlacementErrorAsync(INetworkObject client, FurniturePlacementError error)
    {
        await client.WriteToStreamAsync(new NotificationWriter(NotificationType.FurniturePlacementError,
            new Dictionary<string, string>
            {
                { "message", error.ToString() }
            }).GetAllBytes());
    }

    public static async Task ProcessChatMessageAsync(
        INetworkClient client,
        RoomUserChatEventParser parser,
        bool shouting,
        RoomConstants roomConstants,
        IRoomRepository roomRepository,
        IRoomChatCommandRepository commandRepository)
    {
        var message = parser.Message;
        
        if (string.IsNullOrEmpty(message) || message.Length > roomConstants.MaxChatMessageLength)
        {
            return;
        }
        
        if (!TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        if (!shouting && message.StartsWith(":"))
        {
            var (found, command) = commandRepository.TryGetCommandByTriggerWord(message.Split(" ")[0].Substring(1));

            if (found && command != null)
            {
                await command.ExecuteAsync(roomUser!);
                return;
            }
        }
        
        var chatMessage = new RoomChatMessage(
            roomUser, 
            message, 
            room, 
            parser.Bubble, 
            0, 
            RoomChatMessageType.Shout);
        
        await roomUser.OnTalkAsync(chatMessage);

        await room!.UserRepository.BroadcastDataAsync(
            shouting ? 
            new RoomUserShoutWriter(chatMessage!, 0).GetAllBytes() : 
            new RoomUserChatWriter(chatMessage!, 0).GetAllBytes()
        );
    }
}