namespace Sadie.Networking.Packets.Server.Players.Clothing;

public class PlayerClothingListWriter : NetworkPacketWriter
{
    public PlayerClothingListWriter() : base(ServerPacketId.PlayerClothingList)
    {
        // TODO: Pass structure in 
        
        WriteInt(0);
        WriteInt(0);
    }
}