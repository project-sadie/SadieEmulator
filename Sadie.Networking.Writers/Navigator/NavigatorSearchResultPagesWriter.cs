using Sadie.Game.Navigator;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class NavigatorSearchResultPagesWriter : NetworkPacketWriter
{
    public NavigatorSearchResultPagesWriter(string tabName, string searchQuery, List<NavigatorSearchResult> searchResults)
    {
        WriteShort(ServerPacketId.NavigatorRooms);
        WriteString(tabName);
        WriteString(searchQuery);
        WriteInt(searchResults.Count);

        foreach (var result in searchResults)
        {
            WriteString(result.Unknown1);
            WriteString(result.Unknown2);
            WriteInt((int) result.Action);
            WriteBoolean(result.IsCollapsed);
            WriteInt(0); // TODO: DISPLAY MODE 
            WriteInt(result.Rooms.Count);
            
            foreach (var room in result.Rooms)
            {
                WriteLong(room.Id);
                WriteString(room.Name);
                WriteLong(room.OwnerId);
                WriteString(room.OwnerName);
                WriteInt(0); // TODO: state 
                WriteInt(room.UserRepository.Count);
                WriteInt(50); // TODO: max users
                WriteString("description"); // TODO: Description
                WriteInt(0); // unknown
                WriteInt(456); // TODO: score
                WriteInt(0); // unknown
                WriteInt(1); // TODO: category
                WriteInt(0); // TODO: amount of tags
                // TODO: foreach tags
                WriteInt(0 | 8); // TODO: base
            }
        }
    }
}