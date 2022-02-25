using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Clothing;

public class PlayerClothingListWriter : NetworkPacketWriter
{
    public PlayerClothingListWriter() : base(ServerPacketId.PlayerClothingList)
    {
        // TODO: Complete this
        
        WriteInt(0);
        WriteInt(0);
    }
}