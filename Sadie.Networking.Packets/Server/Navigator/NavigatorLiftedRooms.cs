namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorLiftedRooms : NetworkPacketWriter
{
    public NavigatorLiftedRooms() : base(ServerPacketId.NavigatorLiftedRooms)
    {
        WriteInt(0);
    }
}