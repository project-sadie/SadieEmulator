namespace Sadie.Networking.Packets.Server.Navigator;

internal class NavigatorLiftedRoomsWriter : NetworkPacketWriter
{
    internal NavigatorLiftedRoomsWriter() : base(ServerPacketId.NavigatorLiftedRooms)
    {
        WriteInt(0);
    }
}