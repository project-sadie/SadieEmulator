using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Furniture;

public class RoomFloorFurnitureItemUpdatedWriter : NetworkPacketWriter
{
    public RoomFloorFurnitureItemUpdatedWriter(
        long id,
        int assetId,
        int positionX,
        int positionY,
        double positionZ,
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
        WriteInteger(positionX);
        WriteInteger(positionY);
        WriteInteger(direction);
        WriteString($"{positionZ.ToString():0.00}");
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