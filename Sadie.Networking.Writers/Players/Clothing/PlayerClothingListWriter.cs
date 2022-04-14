using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Clothing;

public class PlayerClothingListWriter : NetworkPacketWriter
{
    // TODO ...
    
    public PlayerClothingListWriter()
    {
        WriteShort(ServerPacketId.PlayerClothingList);
        WriteInteger(0);
        WriteInteger(0);
    }
}