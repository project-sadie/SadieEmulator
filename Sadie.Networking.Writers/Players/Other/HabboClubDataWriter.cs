using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class HabboClubDataWriter : NetworkPacketWriter
{
    public HabboClubDataWriter(int windowId)
    {
        WriteShort(ServerPacketId.HabboClubData);
        WriteInteger(0);
        WriteInteger(windowId);
    }
}