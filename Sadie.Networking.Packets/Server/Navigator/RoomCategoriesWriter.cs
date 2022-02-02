namespace Sadie.Networking.Packets.Server.Navigator;

public class RoomCategoriesWriter : NetworkPacketWriter
{
    public RoomCategoriesWriter() : base(ServerPacketId.RoomCategories)
    {
        WriteInt(0);
    }
}