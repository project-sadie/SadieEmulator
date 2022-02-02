namespace Sadie.Networking.Packets.Server.Players.Friends;

public class MessengerInitComposer : NetworkPacketWriter
{
    public MessengerInitComposer() : base(ServerPacketId.MessengerInitComposer)
    {
        WriteInt(int.MaxValue);
        WriteInt(1337); // unknown1
        WriteInt(int.MaxValue);
        WriteInt(0);
    }
}