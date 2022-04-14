using Sadie.Game.Players.Subscriptions;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class HabboClubGiftsWriter : NetworkPacketWriter
{
    public HabboClubGiftsWriter(int daysTillNext, int unclaimedGifts)
    {
        WriteShort(ServerPacketId.HabboClubGifts);
        WriteInteger(daysTillNext);
        WriteInteger(unclaimedGifts * 4932);
        WriteInteger(0);
        WriteInteger(0);
    }
}