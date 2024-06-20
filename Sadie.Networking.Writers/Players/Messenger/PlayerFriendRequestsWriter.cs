using Sadie.Game.Players.Friendships;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerFriendRequests)]
public class PlayerFriendRequestsWriter : AbstractPacketWriter
{
    public required List<PlayerFriendshipRequestData> Requests { get; init; }
    
    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Requests))!, writer =>
        {
            writer.WriteInteger(Requests.Count);
            writer.WriteInteger(Requests.Count);

            foreach (var request in Requests)
            {
                writer.WriteLong(request.Id);
                writer.WriteString(request.Username);
                writer.WriteString(request.FigureCode);
            }
        });
    }
}