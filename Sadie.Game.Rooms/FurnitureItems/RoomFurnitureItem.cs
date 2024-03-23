using Sadie.Game.Furniture;
using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms.FurnitureItems;

public class RoomFurnitureItem(
    long id,
    long roomId,
    long ownerId,
    string ownerUsername,
    FurnitureItem furnitureItem,
    HPoint position,
    string wallPosition,
    HDirection direction,
    string limitedData,
    string metaData,
    DateTime createdAt)
{
    public long Id { get; set; } = id;
    public long RoomId { get; } = roomId;
    public long OwnerId { get; } = ownerId;
    public string OwnerUsername { get; } = ownerUsername;
    public FurnitureItem FurnitureItem { get; } = furnitureItem;
    public HPoint Position { get; set; } = position;
    public string WallPosition { get; set; } = wallPosition;
    public HDirection Direction { get; set; } = direction;
    public string LimitedData { get; } = limitedData;
    public string MetaData { get; } = metaData;
    public DateTime Created { get; } = createdAt;
}