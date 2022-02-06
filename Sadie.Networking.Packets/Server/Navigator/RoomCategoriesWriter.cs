namespace Sadie.Networking.Packets.Server.Navigator;

internal class RoomCategoriesWriter : NetworkPacketWriter
{
    internal RoomCategoriesWriter() : base(ServerPacketId.RoomCategories)
    {
        WriteInt(0);
    }
}