using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Camera;

public class CameraPriceWriter : NetworkPacketWriter
{
    public CameraPriceWriter(int costCredits, int costPoints, int pointsType)
    {
        WriteShort(ServerPacketId.CameraPrice);
        WriteInteger(costCredits);
        WriteInteger(costPoints);
        WriteInteger(pointsType);
    }
}