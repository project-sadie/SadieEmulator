namespace Sadie.Networking.Packets.Server.Players.Clothing;

public class PlayerClothingList : NetworkPacketWriter
{
    public PlayerClothingList() : base(ServerPacketId.PlayerClothingList)
    {
        WriteInt(0);
        WriteInt(0);
    }
}