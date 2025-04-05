using Sadie.API;
using Sadie.API.Networking;
using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerSearchResult)]
public class PlayerSearchResultWriter : AbstractPacketWriter
{
    public required ICollection<Player> Friends { get; init; }
    public required ICollection<Player> Strangers { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(Friends.Count);

        foreach (var friend in Friends)
        {
            writer.WriteLong(friend.Id);
            writer.WriteString(friend.Username);
            writer.WriteString(friend.AvatarData.Motto);
            writer.WriteBool(false);
            writer.WriteBool(false);
            writer.WriteString("");
            writer.WriteInteger(1);
            writer.WriteString(friend.AvatarData.FigureCode);
            writer.WriteString("");
        }
        
        writer.WriteInteger(Strangers.Count);

        foreach (var stranger in Strangers)
        {
            writer.WriteLong(stranger.Id);
            writer.WriteString(stranger.Username);
            writer.WriteString(stranger.AvatarData.Motto);
            writer.WriteBool(false);
            writer.WriteBool(false);
            writer.WriteString("");
            writer.WriteInteger(1);
            writer.WriteString(stranger.AvatarData.FigureCode);
            writer.WriteString("");
        }
    }
}