namespace Sadie.Networking.Packets.Server.Players.Rooms;

public class PlayerHomeRoomWriter : NetworkPacketWriter
{
    public PlayerHomeRoomWriter(long homeRoom, int newRoom) : base(ServerPacketId.PlayerHomeRoom)
    {
        // TODO: Pass structure in 
        
        WriteLong(homeRoom);
        WriteInt(newRoom);
    }
}