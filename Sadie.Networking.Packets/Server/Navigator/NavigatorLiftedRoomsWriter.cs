namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorLiftedRoomsWriter : NetworkPacketWriter
{
    public NavigatorLiftedRoomsWriter() : base(ServerPacketId.NavigatorLiftedRooms)
    {
        // TODO: Pass structure in 
        
        WriteInt(0);
    }
}