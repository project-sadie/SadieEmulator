namespace Sadie.Networking.Packets.Server.GameCentre;

internal class GameAchievementsListWriter : NetworkPacketWriter
{
    internal GameAchievementsListWriter() : base(ServerPacketId.GameCentreConfig)
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