using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Game.Players.RoomVisits;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Game.Rooms.Enums;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users.Chat;
using Sadie.Networking.Writers;
using Sadie.Networking.Writers.Generic;
using Sadie.Networking.Writers.Players.Purse;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Doorbell;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Events;

internal static class NetworkPacketEventHelpers
{
    internal static bool TryResolveRoomObjectsForClient(
        RoomRepository roomRepository, 
        INetworkClient client, out RoomLogic room, out IRoomUser user)
    {
        var player = client.Player;
        
        if (player == null)
        {
            room = null!;
            user = null!;
            
            return false;
        }
        
        var roomId = player.CurrentRoomId;
        var roomObject = roomRepository.TryGetRoomById(roomId);

        if (roomObject == null || client.RoomUser == null)
        {
            room = null!;
            user = null!;
            
            return false;
        }

        room = roomObject;
        user = client.RoomUser;
        
        return true;
    }

    internal static async Task EnterRoomAsync<T>(
        INetworkClient client, 
        RoomLogic room, 
        ILogger<T> logger, 
        RoomUserFactory roomUserFactory)
    {
        var player = client.Player;
        var playerState = player.State;
        
        player.CurrentRoomId = room.Id;
        playerState.RoomVisits.Add(new PlayerRoomVisit(player.Id, room.Id));

        var controllerLevel = RoomControllerLevel.None;
        
        if (room.PlayerRights.FirstOrDefault(x => x.PlayerId == player.Id) != null)
        {
            controllerLevel = RoomControllerLevel.Rights;
        }

        if (room.OwnerId == player.Id)
        {
            controllerLevel = RoomControllerLevel.Owner;
        }
        
        var roomUser = roomUserFactory.Create(
            room,
            player.NetworkObject,
            player.Id,
            new HPoint(room.Layout.DoorX, room.Layout.DoorY, room.Layout.DoorZ),
            room.Layout.DoorDirection,
            room.Layout.DoorDirection,
            player,
            controllerLevel);

        roomUser.ApplyFlatCtrlStatus();
        
        if (!room.UserRepository.TryAdd(roomUser))
        {
            logger.LogError($"Failed to add user {player.Id} to room {room.Id}");
            return;
        }
        
        client.RoomUser = roomUser;

        var canLikeRoom = player.RoomLikes.FirstOrDefault(x => x.RoomId == room.Id) == null;
        
        await client.WriteToStreamAsync(new RoomDataWriter(room.Id, room.Layout.Name).GetAllBytes());

        if (room.PaintSettings.FloorPaint != "0.0")
        {
            await client.WriteToStreamAsync(new RoomPaintWriter("floor", room.PaintSettings.FloorPaint).GetAllBytes());
        }

        if (room.PaintSettings.WallPaint != "0.0")
        {
            await client.WriteToStreamAsync(new RoomPaintWriter("wallpaper", room.PaintSettings.WallPaint).GetAllBytes());
        }
        
        await client.WriteToStreamAsync(new RoomPaintWriter("landscape", room.PaintSettings.LandscapePaint).GetAllBytes());
        
        await client.WriteToStreamAsync(new RoomScoreWriter(room.PlayerLikes.Count, canLikeRoom).GetAllBytes());
        await client.WriteToStreamAsync(new RoomPromotionWriter().GetAllBytes());

        var owner = room.OwnerId == player.Id;
        
        await client.WriteToStreamAsync(new RoomWallFloorSettingsWriter(
            room.Settings.HideWalls, 
            room.Settings.WallThickness, 
            room.Settings.FloorThickness).GetAllBytes());
        
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
        ServerRoomConstants roomConstants,
        RoomRepository roomRepository,
        IRoomChatCommandRepository commandRepository,
        SadieContext dbContext)
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
            var command = commandRepository
                .TryGetCommandByTriggerWord(message.Split(" ")[0]
                .Substring(1));

            if (command != null)
            {
                await command.ExecuteAsync(roomUser!);
                return;
            }
        }

        var chatMessage = new RoomChatMessage
        {
            RoomId = room.Id,
            PlayerId = roomUser.Id,
            Message = message,
            ChatBubbleId = parser.Bubble,
            EmotionId = 0,
            TypeId = RoomChatMessageType.Shout,
            CreatedAt = DateTime.Now
        };

        await room!.UserRepository.BroadcastDataAsync(
            shouting ? 
            new RoomUserShoutWriter(chatMessage!, 0).GetAllBytes() : 
            new RoomUserChatWriter(chatMessage!, 0).GetAllBytes()
        );
        
        roomUser.UpdateLastAction();

        room.ChatMessages.Add(chatMessage);

        dbContext.Add(chatMessage);
        await dbContext.SaveChangesAsync();
    }

    public static async Task<bool> ValidateRoomAccessForClientAsync(INetworkClient client, RoomLogic room, string password)
    {
        var player = client.Player!;
        
        switch (room.Settings.AccessType)
        {
            case RoomAccessType.Password when password != room.Settings.Password:
                await client.WriteToStreamAsync(new GenericErrorWriter(GenericErrorCode.IncorrectRoomPassword).GetAllBytes());
                await client.WriteToStreamAsync(new PlayerHotelViewWriter().GetAllBytes());
                return false;
            case RoomAccessType.Doorbell:
            {
                var usersWithRights = room.UserRepository.GetAllWithRights();
                
                if (usersWithRights.Count < 1)
                {
                    await client.WriteToStreamAsync(new RoomDoorbellNoAnswerWriter(player.Username).GetAllBytes());
                }
                else
                {
                    foreach (var user in usersWithRights)
                    {
                        await user.NetworkObject.WriteToStreamAsync(new RoomDoorbellWriter(player.Username)
                            .GetAllBytes());
                    }

                    await client.WriteToStreamAsync(new RoomDoorbellWriter().GetAllBytes());
                }

                return false;
            }
        }

        return true;
    }

    public static async Task<bool> TryChargeForCatalogItemPurchaseAsync(INetworkClient client, CatalogItem item, int amount)
    {
        var costInCredits = item.CostCredits * amount;
        var costInPoints = item.CostPoints * amount;

        var playerData = client.Player.Data;
        
        if (playerData.CreditBalance < costInCredits || 
            (item.CostPointsType == 0 && playerData.PixelBalance < costInPoints) ||
            (item.CostPointsType != 0 && playerData.SeasonalBalance < costInPoints))
        {
            return false;
        }

        playerData.CreditBalance -= costInCredits;

        if (item.CostPointsType == 0)
        {
            playerData.PixelBalance -= costInPoints;
        }
        else
        {
            playerData.SeasonalBalance -= costInPoints;
        }

        var currencies = new Dictionary<int, long>
        {
            {0, playerData.PixelBalance},
            {1, 0}, // snowflakes
            {2, 0}, // hearts
            {3, 0}, // gift points
            {4, 0}, // shells
            {5, playerData.SeasonalBalance},
            {101, 0}, // snowflakes
            {102, 0}, // unknown
            {103, playerData.GotwPoints},
            {104, 0}, // unknown
            {105, 0} // unknown
        };
        
        await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter(playerData.CreditBalance).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter(currencies).GetAllBytes());

        return true;
    }

    public static string CalculateMetaDataForCatalogItem(string metaData, CatalogItem item)
    {
        switch (item.FurnitureItems.First().InteractionType)
        {
            case "roomeffect":
                if (string.IsNullOrEmpty(metaData))
                {
                    return 0.ToString();
                }

                return double
                    .Parse(metaData)
                    .ToString()
                    .Replace(',', '.');
            default:
                return metaData;
        }
    }
}