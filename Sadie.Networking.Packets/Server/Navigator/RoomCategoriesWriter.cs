namespace Sadie.Networking.Packets.Server.Navigator;

public class RoomCategoriesWriter : NetworkPacketWriter
{
    public RoomCategoriesWriter() : base(ServerPacketId.RoomCategories)
    {
        // TODO: Pass structure in 
        
        WriteInt(0);
    }
}