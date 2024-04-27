using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerFriendRequests)]
public class PlayerFriendRequestsWriter : AbstractPacketWriter
{
    public required List<Player> Requests { get; init; }
    
    public override void OnConfigureRules()
    {
        Override(PropertyHelper<PlayerFriendRequestsWriter>.GetProperty(x => x.Requests), writer =>
        {
            writer.WriteInteger(Requests.Count);
            writer.WriteInteger(Requests.Count);

            foreach (var request in Requests)
            {
                writer.WriteLong(request.Id);
                writer.WriteString(request.Username);
                writer.WriteString(request.AvatarData.FigureCode);
            }
        });
    }
}