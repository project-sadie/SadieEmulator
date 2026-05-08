using Sadie.API.DTOs.Players;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Navigator;

[PacketId(ServerPacketId.NavigatorSavedSearches)]
public class PlayerSavedSearchesWriter : AbstractPacketWriter
{
    public required ICollection<PlayerSavedSearchDto> Searches { get; init; }
}