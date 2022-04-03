using Sadie.Game.Players;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerProfileWriter : NetworkPacketWriter
{
    public PlayerProfileWriter(IPlayerData playerData)
    {
        WriteShort(ServerPacketId.PlayerProfile);
        WriteLong(playerData.Id);
        WriteString(playerData.Username);
        WriteString(playerData.FigureCode);
        WriteString(playerData.Motto);
        WriteString("IDontFuckingKnow");
        WriteLong(playerData.AchievementScore);
        WriteLong(0); // Friendship count
        WriteBoolean(false); // Is the current user a friend?
        WriteBoolean(false); // Has the current user sent a friend request?
        WriteBoolean(true); // Are they online?
        WriteInt(0);
        WriteInt(1);
        WriteBoolean(true);
    }
}