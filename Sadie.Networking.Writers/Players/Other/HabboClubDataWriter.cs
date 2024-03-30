using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class HabboClubDataWriter : NetworkPacketWriter
{
    public HabboClubDataWriter(int windowId)
    {
        WriteShort(ServerPacketId.HabboClubData);
        WriteInteger(1);

        WriteInteger(1);
        WriteString("Testing Offer");
        WriteBool(false);
        WriteInteger(1);
        WriteInteger(1);
        WriteInteger(1);
        WriteBool(true);
        WriteInteger(1);
        WriteInteger(1);
        WriteBool(false);
        WriteInteger(1);
        WriteInteger(2023);
        WriteInteger(3);
        WriteInteger(1);
        
        WriteInteger(windowId);
    }
}