using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Camera;

public class CameraPriceWriter : AbstractPacketWriter
{
    public CameraPriceWriter(
        int costCredits, 
        int costPoints, 
        int pointsType)
    {
        WriteShort(ServerPacketId.CameraPrice);
        WriteInteger(costCredits);
        WriteInteger(costPoints);
        WriteInteger(pointsType);
    }
}