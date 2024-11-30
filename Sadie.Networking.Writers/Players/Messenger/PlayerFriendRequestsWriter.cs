using Sadie.API.Game.Players.Friendships;
using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerFriendRequests)]
public class PlayerFriendRequestsWriter : AbstractPacketWriter
{
    public required List<IPlayerFriendshipRequestData> Requests { get; init; }
    
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