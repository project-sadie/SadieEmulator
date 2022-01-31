namespace Sadie.Networking.Packets.Server.Navigator;

public class RoomCategories : NetworkPacketWriter
{
    public RoomCategories() : base(ServerPacketIds.RoomCategories)
    {
        WriteInt(0);
    }
}