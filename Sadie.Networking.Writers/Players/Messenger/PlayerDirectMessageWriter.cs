using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization;
using Sadie.Shared.Extensions;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerMessage)]
public class PlayerDirectMessageWriter : AbstractPacketWriter
{
    public required PlayerMessage Message { get; init; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(Message.OriginPlayerId);
        writer.WriteString(Message.Message);
        writer.WriteLong(DateTime.Now.ToUnix() - Message.CreatedAt.ToUnix());
    }
}