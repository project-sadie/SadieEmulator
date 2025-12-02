using Sadie.API;
using Sadie.API.DTOs.Player;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;
using Sadie.Core.Shared.Extensions;

namespace Sadie.Networking.Packets.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerMessage)]
public class PlayerDirectMessageWriter : AbstractPacketWriter
{
    public required PlayerMessageDto Message { get; init; }

    public override async Task OnSerializeAsync(INetworkPacketWriter writer)
    {
        writer.WriteLong(Message.OriginPlayerId);
        writer.WriteString(Message.Message ?? "");
        writer.WriteLong(DateTime.Now.ToUnix() - Message.CreatedAt.ToUnixTimeSeconds());
    }
}