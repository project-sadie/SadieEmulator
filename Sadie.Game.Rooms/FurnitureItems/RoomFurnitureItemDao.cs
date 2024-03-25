using Sadie.Database;
using Sadie.Game.Furniture;
using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms.FurnitureItems;

public class RoomFurnitureItemDao(
    IDatabaseProvider databaseProvider, 
    FurnitureItemRepository furnitureRepository)
    : BaseDao(databaseProvider), IRoomFurnitureItemDao
{

    public async Task<int> CreateItemAsync(RoomFurnitureItem item)
    {
        var parameters = new Dictionary<string, object>
        {
            { "roomId", item.RoomId },
            { "ownerId", item.OwnerId },
            { "ownerUsername", item.OwnerUsername },
            { "furnitureId", item.FurnitureItem.Id },
            { "positionX", item.Position.X },
            { "positionY", item.Position.Y },
            { "positionZ", item.Position.Z },
            { "wallPosition", item.WallPosition },
            { "direction", item.Direction },
            { "limitedData", item.LimitedData },
            { "extraData", item.MetaData },
            { "createdAt", item.Created }
        };

        return await QueryScalarAsync(@"INSERT INTO room_furniture_items (
              room_id, 
              owner_player_id, 
              owner_player_username, 
              furniture_item_id, 
              position_x, 
              position_y, 
              position_z, 
              wall_position,
              direction, limited_data, meta_data, created_at
            )
            VALUES (
                    @roomId, 
                    @ownerId, 
                    @ownerUsername, 
                    @furnitureId, 
                    @positionX, 
                    @positionY, 
                    @positionZ, 
                    @wallPOsition,
                    @direction, 
                    @limitedData, 
                    @extraData, 
                    @createdAt); 
            SELECT LAST_INSERT_ID();", parameters);
    }

    public async Task<List<RoomFurnitureItem>> GetItemsForRoomAsync(int roomId)
    {
        var reader = await GetReaderAsync(@"
            SELECT 
                id, 
                room_id, 
                owner_player_id, 
                owner_player_username, 
                furniture_item_id, 
                position_x, 
                position_y, 
                position_z, 
                wall_position,
                direction, 
                limited_data, 
                meta_data, 
                created_at FROM room_furniture_items
            WHERE room_id = @roomId", new Dictionary<string, object>
        {
            { "roomId", roomId }
        });
        
        var data = new List<RoomFurnitureItem>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(CreateItemFromRecord(record));
        }
        
        return data;
    }

    public async Task DeleteItemsAsync(List<long> itemIds)
    {
        await QueryAsync($"DELETE FROM room_furniture_items WHERE id IN ({string.Join(",", itemIds)});");
    }

    public async Task UpdateItemAsync(RoomFurnitureItem item)
    {
        await QueryAsync(@"UPDATE room_furniture_items SET 
                position_x = @x, 
                position_y = @y, 
                position_z = @z, 
                wall_position = @wallPosition, 
                direction = @direction WHERE id = @id LIMIT 1;", new Dictionary<string, object>
        {
            { "id", item.Id },
            { "x", item.Position.X },
            { "y", item.Position.Y },
            { "z", item.Position.Z },
            { "wallPosition", item.WallPosition },
            { "direction", (int) item.Direction },
        });
    }

    private RoomFurnitureItem CreateItemFromRecord(DatabaseRecord record)
    {
        var point = new HPoint(
            record.Get<int>("position_x"),
            record.Get<int>("position_y"),
            record.Get<float>("position_z"));

        var (found, furnitureItem) = furnitureRepository.TryGetById(
            record.Get<int>("furniture_item_id"));

        if (!found || furnitureItem == null)
        {
            throw new Exception($"Failed to resolve furniture item from room item {record.Get<long>("id")}");
        }
        
        return new RoomFurnitureItem(
            record.Get<int>("id"),
            record.Get<int>("room_id"),
            record.Get<int>("owner_player_id"),
            record.Get<string>("owner_player_username"),
            furnitureItem,
            point,
            record.Get<string>("wall_position"),
            (HDirection)record.Get<int>("direction"),
            record.Get<string>("limited_data"),
            record.Get<string>("meta_data"),
            record.Get<DateTime>("created_at"));
    }
}