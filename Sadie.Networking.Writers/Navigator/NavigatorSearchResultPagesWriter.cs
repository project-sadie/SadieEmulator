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
        
        WriteInt(resultCategories.Count);

        foreach (var result in resultCategories)
        {
            WriteString(result.CodeName);
            WriteString(result.Name);
            WriteInt((int) 0); // TODO: SEARCH ACTION?
            WriteBoolean(false); // TODO: is it collapsed?
            WriteInt(0); // TODO: Show thumbnail? display mode

            var rooms = roomProvider.GetRoomsForCategoryName(result.CodeName);
            
            WriteInt(rooms.Count);
            
            foreach (var room in rooms)
            {
                WriteLong(room.Id);
                WriteString(room.Name);
                WriteLong(room.OwnerId);
                WriteString(room.OwnerName);
                WriteInt(0); // TODO: state 
                WriteInt(room.UserRepository.Count);
                WriteInt(room.MaxUsers);
                WriteString(room.Description);
                WriteInt(0); // unknown
                WriteInt(room.Score);
                WriteInt(0); // unknown
                WriteInt(1); // TODO: category
                WriteInt(room.Tags.Count);

                foreach (var tag in room.Tags)
                {
                    WriteString(tag);
                }
                
                WriteInt(0 | 8); // TODO: base
            }
        }
    }
}