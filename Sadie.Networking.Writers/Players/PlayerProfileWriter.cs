using Sadie.Game.Players;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerProfileWriter : NetworkPacketWriter
{
    public PlayerProfileWriter(
        IPlayerData playerData, 
        bool online, 
        int friendshipCount, 
        bool friendshipExists, 
        bool friendshipRequestExists)
    {
        WriteShort(ServerPacketId.PlayerProfile);
        WriteLong(playerData.Id);
        WriteString(playerData.Username);
        WriteString(playerData.FigureCode);
        WriteString(playerData.Motto);
        WriteString(playerData.CreatedAt.ToString("dd MMMM yyyy"));
        WriteLong(playerData.AchievementScore);
        WriteLong(friendshipCount);
        WriteBool(friendshipExists);
        WriteBool(friendshipRequestExists);
        WriteBool(online);
        WriteInteger(0);
        WriteInteger((int)(DateTime.Now - playerData.LastOnline).TotalSeconds);
        WriteBool(true);
    }
}