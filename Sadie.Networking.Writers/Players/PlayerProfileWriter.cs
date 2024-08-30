using Sadie.API;
using Sadie.API.Networking;
using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players;

[PacketId(ServerPacketId.PlayerProfile)]
public class PlayerProfileWriter : AbstractPacketWriter
{
    public required Player Player { get; init; }
    public required bool Online { get; init; }
    public required int FriendshipCount { get; init; }
    public required bool FriendshipExists { get; init; }
    public required bool FriendshipRequestExists { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        var lastOnline = Player.Data.LastOnline == null
            ? 0
            : (int) (DateTime.Now - Player.Data.LastOnline).Value.TotalSeconds;
        
        writer.WriteLong(Player.Id);
        writer.WriteString(Player.Username);
        writer.WriteString(Player.AvatarData.FigureCode);
        writer.WriteString(Player.AvatarData.Motto);
        writer.WriteString(Player.CreatedAt.ToString("dd MMMM yyyy"));
        writer.WriteLong(Player.Data.AchievementScore);
        writer.WriteLong(FriendshipCount);
        writer.WriteBool(FriendshipExists);
        writer.WriteBool(FriendshipRequestExists);
        writer.WriteBool(Online);
        writer.WriteInteger(0);
        writer.WriteInteger(lastOnline);
        writer.WriteBool(true);
    }
}