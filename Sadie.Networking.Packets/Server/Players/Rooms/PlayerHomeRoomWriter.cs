namespace Sadie.Networking.Packets.Server.Players.Rooms;

internal class PlayerHomeRoomWriter : NetworkPacketWriter
{
    internal PlayerHomeRoomWriter(long homeRoom, int newRoom) : base(ServerPacketId.PlayerHomeRoom)
    {
        WriteLong(homeRoom);
        WriteInt(newRoom);
    }
}