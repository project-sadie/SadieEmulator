using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

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