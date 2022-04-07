using Sadie.Game.Players;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerProfileWriter : NetworkPacketWriter
{
    public PlayerProfileWriter(IPlayerData playerData, bool online, int friendshipCount, bool friendshipExists, bool friendshipRequestExists)
    {
        WriteShort(ServerPacketId.PlayerProfile);
        WriteLong(playerData.Id);
        WriteString(playerData.Username);
        WriteString(playerData.FigureCode);
        WriteString(playerData.Motto);
        WriteString(playerData.CreatedAt.ToString("yyyy-MM-dd HH:mm:s"));
        WriteLong(playerData.AchievementScore);
        WriteLong(friendshipCount);
        WriteBoolean(friendshipExists);
        WriteBoolean(friendshipRequestExists);
        WriteBoolean(online);
        WriteInt(0);
        WriteInt(1);
        WriteBoolean(true);
    }
}