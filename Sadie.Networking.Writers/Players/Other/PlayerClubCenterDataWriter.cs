using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerClubCenterDataWriter : NetworkPacketWriter
{
    public PlayerClubCenterDataWriter(DateTime firstJoined, int streakInDays)
    {
        WriteShort(ServerPacketId.HabboClubCenter);
        WriteInteger(streakInDays);
        WriteString(firstJoined.ToString("dd/MM/yyyy"));
        WriteInteger(1);
        WriteInteger(2);
        WriteInteger(3);
        WriteInteger(4);
        WriteInteger(5);
        WriteInteger(200);
        WriteInteger(10);
        WriteInteger(100000);
    }
}