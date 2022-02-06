namespace Sadie.Networking.Packets.Server.Players.Friends;

internal class PlayerMessengerInitWriter : NetworkPacketWriter
{
    internal PlayerMessengerInitWriter() : base(ServerPacketId.PlayerMessengerInit)
    {
        WriteInt(int.MaxValue);
        WriteInt(1337); // unknown1
        WriteInt(int.MaxValue);
        WriteInt(0);
    }
}