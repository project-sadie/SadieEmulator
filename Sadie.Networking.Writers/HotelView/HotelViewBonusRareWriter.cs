using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.HotelView;

public class HotelViewBonusRareWriter : NetworkPacketWriter
{
    public HotelViewBonusRareWriter(
        string name, 
        int id, 
        int coins, 
        int coinsRequiredToBuy)
    {
        WriteShort(ServerPacketId.HotelViewBonusRare);
        WriteString(name);
        WriteInteger(id);
        WriteInteger(coins);
        WriteInteger(coinsRequiredToBuy);
    }
}