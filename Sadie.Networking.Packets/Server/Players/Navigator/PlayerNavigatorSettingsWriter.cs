namespace Sadie.Networking.Packets.Server.Players.Navigator;

internal class PlayerNavigatorSettingsWriter : NetworkPacketWriter
{
    internal PlayerNavigatorSettingsWriter(int windowPositionX, int windowPositionY, int windowWidth, int windowHeight, bool openSearches, int unknown1) : base(ServerPacketId.NavigatorSettings)
    {
        WriteInt(windowPositionX);
        WriteInt(windowPositionY);
        WriteInt(windowWidth);
        WriteInt(windowHeight);
        WriteBoolean(openSearches);
        WriteInt(unknown1);
    }
}