namespace Sadie.Networking.Packets.Server.Players.Purse;

public class PlayerActivityPointsBalance : NetworkPacketWriter
{
    public PlayerActivityPointsBalance(long pixels, long seasonPoints, long gotwPoints) : base(ServerPacketId.PlayerPermissions)
    {
        
        WriteInt(11);//Count
        WriteInt(0);//Pixels
        WriteLong(pixels);
        WriteInt(1);//Snowflakes
        WriteInt(16);
        WriteInt(2);//Hearts
        WriteInt(15);
        WriteInt(3);//Gift points
        WriteInt(14);
        WriteInt(4);//Shells
        WriteInt(13);
        WriteInt(5);//Diamonds
        WriteLong(seasonPoints);
        WriteInt(101);//Snowflakes
        WriteInt(10);
        WriteInt(102);
        WriteInt(0);
        WriteInt(103);//Stars
        WriteLong(gotwPoints);
        WriteInt(104);//Clouds
        WriteInt(0);
        WriteInt(105);//Diamonds
        WriteInt(0);
    }
}