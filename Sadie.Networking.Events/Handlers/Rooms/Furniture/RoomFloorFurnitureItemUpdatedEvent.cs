using Sadie.Game.Rooms;
using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Furniture;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

public class RoomFloorFurnitureItemUpdatedEvent(
    RoomFloorFurnitureItemUpdatedParser parser,
    IRoomRepository roomRepository, 
    IRoomFurnitureItemDao roomFurnitureItemDao) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }

        var itemId = reader.ReadInteger();
        
        var (found, room) = roomRepository.TryGetRoomById(client.Player.Data.CurrentRoomId);
        
        if (!found || room == null || !client.RoomUser.HasRights())
        {
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            await PacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.MissingRights);
            return;
        }

        var roomFurnitureItem = room.FurnitureItemRepository.Items.FirstOrDefault(x => x.Id == itemId);

        if (roomFurnitureItem == null)
        {
            return;
        }
        
        var currentTile = room.Layout.FindTile(
            roomFurnitureItem.Position.X, roomFurnitureItem.Position.Y);

        currentTile?.Items.Remove(roomFurnitureItem);
        
        if (currentTile != null)
        {
            RoomHelpers.UpdateTileMapForTile(currentTile, room.Layout);
        }

        var position = new HPoint(
            reader.ReadInteger(),
            reader.ReadInteger(),
            roomFurnitureItem.Position.Z);
        
        var direction = (HDirection) reader.ReadInteger();
        var tile = room.Layout.FindTile(position.X, position.Y);

        if (tile == null || tile.State == RoomTileState.Closed)
        {
            await PacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        tile.Items.Add(roomFurnitureItem);
        RoomHelpers.UpdateTileMapForTile(tile, room.Layout);

        roomFurnitureItem.Position = position;
        roomFurnitureItem.Direction = direction;
        
        await roomFurnitureItemDao.UpdateItemAsync(roomFurnitureItem);
        await room.UserRepository.BroadcastDataAsync(new RoomFloorFurnitureItemUpdatedWriter(
            roomFurnitureItem.Id,
            roomFurnitureItem.FurnitureItem.AssetId,
            roomFurnitureItem.Position,
            (int) roomFurnitureItem.Direction,
            0,
            1,
            (int) ObjectDataKey.MapKey,
            new Dictionary<string, string>(),
            roomFurnitureItem.FurnitureItem.InteractionType,
            roomFurnitureItem.MetaData,
            roomFurnitureItem.FurnitureItem.InteractionModes,
            -1,
            roomFurnitureItem.OwnerId).GetAllBytes());
    }

    public RoomFloorFurnitureItemUpdatedEvent(IRoomRepository roomRepository, 
        IRoomFurnitureItemDao roomFurnitureItemDao) : this(default(RoomFloorFurnitureItemUpdatedParser),
        roomRepository,
        roomFurnitureItemDao)
    {
    }
}