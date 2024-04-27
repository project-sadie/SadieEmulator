using Sadie.Database.Models.Players;
using Sadie.Shared.Extensions;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerMessage)]
public class PlayerDirectMessageWriter : AbstractPacketWriter
{
    public required PlayerMessage Message { get; init; }

    public override void OnConfigureRules()
    {
        Override(PropertyHelper<PlayerDirectMessageWriter>.GetProperty(x => x.Message.CreatedAt), writer =>
        {
            writer.WriteLong(DateTime.Now.ToUnix() - Message.CreatedAt.ToUnix());
        });
    }
}