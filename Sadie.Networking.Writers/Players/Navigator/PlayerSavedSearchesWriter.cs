using Sadie.Game.Players.Navigator;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Navigator;

public class PlayerSavedSearchesWriter : NetworkPacketWriter
{
    public PlayerSavedSearchesWriter(List<PlayerSavedSearch> searches)
    {
        WriteShort(ServerPacketId.NavigatorSavedSearches);
        WriteInteger(searches.Count);

        foreach (var search in searches)
        {
            WriteLong(search.Id);
            WriteString(search.Search);
            WriteString(search.Filter);
            WriteString(search.Unknown1);
        }
    }
}