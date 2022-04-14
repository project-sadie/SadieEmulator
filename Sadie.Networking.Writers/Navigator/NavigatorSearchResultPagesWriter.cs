using Sadie.Game.Navigator;
using Sadie.Game.Navigator.Categories;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class NavigatorSearchResultPagesWriter : NetworkPacketWriter
{
    public NavigatorSearchResultPagesWriter(string tabName, string searchQuery, List<NavigatorCategory> resultCategories, NavigatorRoomProvider roomProvider)
    {
        WriteShort(ServerPacketId.NavigatorRooms);
        WriteString(tabName);
        WriteString(searchQuery);
        
        WriteInteger(resultCategories.Count);

        foreach (var result in resultCategories)
        {
            WriteString(result.CodeName);
            WriteString(result.Name);
            WriteInteger((int) 0); // TODO: SEARCH ACTION?
            WriteBool(false); // TODO: is it collapsed?
            WriteInteger(0); // TODO: Show thumbnail? display mode

            var rooms = roomProvider.GetRoomsForCategoryName(result.CodeName);
            
            WriteInteger(rooms.Count);
            
            foreach (var room in rooms)
            {
                WriteLong(room.Id);
                WriteString(room.Name);
                WriteLong(room.OwnerId);
                WriteString(room.OwnerName);
                WriteInteger(0); // TODO: state 
                WriteInteger(room.UserRepository.Count);
                WriteInteger(room.MaxUsers);
                WriteString(room.Description);
                WriteInteger(0); // unknown
                WriteInteger(room.Score);
                WriteInteger(0); // unknown
                WriteInteger(1); // TODO: category
                WriteInteger(room.Tags.Count);

                foreach (var tag in room.Tags)
                {
                    WriteString(tag);
                }
                
                WriteInteger(0 | 8); // TODO: base
            }
        }
    }
}