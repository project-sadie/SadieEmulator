using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomFloorFurnitureItemUpdatedWriter : NetworkPacketWriter
{
    public RoomFloorFurnitureItemUpdatedWriter(
        long id,
        int assetId,
        HPoint point,
        int direction,
        int stackHeight,
        int extra,
        int objectDataKey,
        Dictionary<string, string> objectData,
        string interactionType,
        string metaData,
        int interactionModes,
        int expires,
        long ownerId)
    {
        WriteShort(ServerPacketId.RoomFloorFurnitureItemMoved);
        WriteLong(id);
        WriteInteger(assetId);
        WriteInteger(point.X);
        WriteInteger(point.Y);
        WriteInteger(direction);
        WriteString($"{point.Z.ToString():0.00}");
        WriteString($"{stackHeight}");
        WriteInteger(extra);

        switch (interactionType)
        {
            default:
                WriteInteger(objectDataKey); 
                WriteInteger(objectData.Count);

                foreach (var dataPair in objectData)
                {
                    WriteString(dataPair.Key);
                    WriteString(dataPair.Value);
                }
                break;
        }

        WriteString(metaData);
        WriteInteger(expires);
        WriteInteger(interactionModes > 1 ? 1 : 0); 
        WriteLong(ownerId);
    }
}