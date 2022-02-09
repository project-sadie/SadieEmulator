using Sadie.Game.Players.Navigator;

namespace Sadie.Networking.Packets.Server.Players.Navigator;

internal class PlayerSavedSearchesWriter : NetworkPacketWriter
{
    internal PlayerSavedSearchesWriter(List<PlayerSavedSearch> searches) : base(ServerPacketId.NavigatorSavedSearches)
    {
        WriteInt(searches.Count);

        foreach (var search in searches)
        {
            WriteLong(search.Id);
            WriteString(search.Search);
            WriteString(search.Filter);
            WriteString(search.Unknown1);
        }
    }
}