using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players.Navigator;

[PacketId(ServerPacketId.NavigatorSavedSearches)]
public class PlayerSavedSearchesWriter : AbstractPacketWriter
{
    public required ICollection<PlayerSavedSearch> Searches { get; init; }

    public override void OnConfigureRules()
    {
        Override(PropertyHelper<PlayerSavedSearchesWriter>.GetProperty(x => x.Searches), writer =>
        {
            writer.WriteInteger(Searches.Count);

            foreach (var search in Searches)
            {
                writer.WriteLong(search.Id);
                writer.WriteString(search.Search);
                writer.WriteString(search.Filter);
                writer.WriteString("");
            }
        });
    }
}