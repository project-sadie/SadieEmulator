namespace Sadie.Networking.Packets.Server.Players.Clothing;

internal class PlayerClothingListWriter : NetworkPacketWriter
{
    internal PlayerClothingListWriter() : base(ServerPacketId.PlayerClothingList)
    {
        WriteInt(0);
        WriteInt(0);
    }
}