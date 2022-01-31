namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorLiftedRooms : NetworkPacketWriter
{
    public NavigatorLiftedRooms() : base(ServerPacketIds.NavigatorLiftedRooms)
    {
        WriteInt(0);
    }
}