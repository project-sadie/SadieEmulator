using Sadie.Game.Players.Subscriptions;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

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