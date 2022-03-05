using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.GameCentre;

public class GameAchievementsListWriter : NetworkPacketWriter
{
    public GameAchievementsListWriter() : base(ServerPacketId.GameCentreConfig)
    {
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(3);
        WriteInt(1);
        WriteInt(1);
        WriteString("BaseJumpBigParachute");
        WriteInt(1);
    }
}