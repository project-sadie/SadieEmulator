namespace Sadie.Networking.Packets.Server.Players.Friends;

public class PlayerMessengerInitWriter : NetworkPacketWriter
{
    public PlayerMessengerInitWriter() : base(ServerPacketId.MessengerInitComposer)
    {
        WriteInt(int.MaxValue);
        WriteInt(1337); // unknown1
        WriteInt(int.MaxValue);
        WriteInt(0);
    }
}