namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorLiftedRoomsWriter : NetworkPacketWriter
{
    public NavigatorLiftedRoomsWriter() : base(ServerPacketId.NavigatorLiftedRooms)
    {
        WriteInt(0);
    }
}