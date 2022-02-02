namespace Sadie.Networking.Packets.Server.Navigator;

public class RoomCategories : NetworkPacketWriter
{
    public RoomCategories() : base(ServerPacketId.RoomCategories)
    {
        WriteInt(0);
    }
}