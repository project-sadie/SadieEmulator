using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Game.Players.Room;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Rooms;
using Sadie.Networking.Writers.Rooms;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Networking.Events;

internal static class PacketEventHelpers
{
    internal static bool TryResolveRoomObjectsForClient(IRoomRepository roomRepository, INetworkClient client, out IRoom room, out RoomUser user)
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

    internal static async Task EnterRoomAsync<T>(INetworkClient client, IRoom room, ILogger<T> logger, IRoomUserFactory roomUserFactory)
    {
        var player = client.Player;
        var playerData = player.Data;
        var playerState = player.State;
        
        playerData.CurrentRoomId = room.Id;
        playerState.RoomVisits.Add(new PlayerRoomVisit(playerData.Id, room.Id));

        var avatarData = (IAvatarData) player.Data;
        
        var roomUser = roomUserFactory.Create(
            room,
            player.NetworkObject,
            playerData.Id,
            room.Layout.DoorPoint,
            room.Layout.DoorDirection,
            room.Layout.DoorDirection,
            avatarData);

        if (!room.UserRepository.TryAdd(roomUser))
        {
            logger.LogError($"Failed to add user {playerData.Id} to room {room.Id}");
            return;
        }
        
        client.RoomUser = roomUser;
        
        await client.WriteToStreamAsync(new RoomDataWriter(room.Id, room.Layout.Name).GetAllBytes());
        await client.WriteToStreamAsync(new RoomPaintWriter("landscape", "0.0").GetAllBytes());
        await client.WriteToStreamAsync(new RoomScoreWriter(room.Score, true).GetAllBytes());
        await client.WriteToStreamAsync(new RoomPromotionWriter().GetAllBytes());

        var owner = room.OwnerId == playerData.Id;
        var rightLevel = owner ? RoomRightLevel.Admin : RoomRightLevel.None;
        
        await client.WriteToStreamAsync(new RoomPaneWriter(room.Id, owner).GetAllBytes());
        await client.WriteToStreamAsync(new RoomRightsWriter(rightLevel).GetAllBytes());
        
        if (owner)
        {
            await client.WriteToStreamAsync(new RoomOwnerWriter().GetAllBytes());
        }
        
        await client.WriteToStreamAsync(new RoomLoadedWriter().GetAllBytes());
    }
}