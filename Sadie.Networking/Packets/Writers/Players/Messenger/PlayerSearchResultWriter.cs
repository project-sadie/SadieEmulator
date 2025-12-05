using Sadie.API;
using Sadie.API.DTOs.Players;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerSearchResult)]
public class PlayerSearchResultWriter : AbstractPacketWriter
{
    public required ICollection<PlayerDto> Friends { get; init; }
    public required ICollection<PlayerDto> Strangers { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(Friends.Count);

        foreach (var friend in Friends)
        {
            writer.WriteLong(friend.Id);
            writer.WriteString(friend.Username);
            writer.WriteString(friend.AvatarData.Motto ?? string.Empty);
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
            writer.WriteString(stranger.AvatarData.Motto ?? string.Empty);
            writer.WriteBool(false);
            writer.WriteBool(false);
            writer.WriteString("");
            writer.WriteInteger(1);
            writer.WriteString(stranger.AvatarData.FigureCode);
            writer.WriteString("");
        }
    }
}