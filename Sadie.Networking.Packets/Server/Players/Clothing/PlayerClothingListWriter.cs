namespace Sadie.Networking.Packets.Server.Players.Clothing;

public class PlayerClothingListWriter : NetworkPacketWriter
{
    public PlayerClothingListWriter() : base(ServerPacketId.PlayerClothingList)
    {
        WriteInt(0);
        WriteInt(0);
    }
}