using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.GameCentre;

public class GameAchievementsListWriter : NetworkPacketWriter
{
    public GameAchievementsListWriter()
    {
        WriteShort(ServerPacketId.GameCentreConfig);
        WriteInteger(0);
        WriteInteger(0);
        WriteInteger(0);
        WriteInteger(0);
        WriteInteger(0);
        WriteInteger(0);
        WriteInteger(0);
        WriteInteger(0);
        WriteInteger(0);
        WriteInteger(0);
        WriteInteger(3);
        WriteInteger(1);
        WriteInteger(1);
        WriteString("BaseJumpBigParachute");
        WriteInteger(1);
    }
}