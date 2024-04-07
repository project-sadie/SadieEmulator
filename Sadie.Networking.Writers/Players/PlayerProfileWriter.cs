using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerProfileWriter : NetworkPacketWriter
{
    public PlayerProfileWriter(
        Player player, 
        bool online, 
        int friendshipCount, 
        bool friendshipExists, 
        bool friendshipRequestExists)
    {
        WriteShort(ServerPacketId.PlayerProfile);
        WriteLong(player.Id);
        WriteString(player.Username);
        WriteString(player.AvatarData.FigureCode);
        WriteString(player.AvatarData.Motto);
        WriteString(player.CreatedAt.ToString("dd MMMM yyyy"));
        WriteLong(player.Data.AchievementScore);
        WriteLong(friendshipCount);
        WriteBool(friendshipExists);
        WriteBool(friendshipRequestExists);
        WriteBool(online);
        WriteInteger(0);
        WriteInteger((int)(DateTime.Now - player.Data.LastOnline).TotalSeconds);
        WriteBool(true);
    }
}