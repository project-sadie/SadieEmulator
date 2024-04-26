using Sadie.Database.Models.Navigator;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class NavigatorSearchResultPagesWriter : AbstractPacketWriter
{
    public NavigatorSearchResultPagesWriter(
        string tabName, 
        string searchQuery, 
        Dictionary<NavigatorCategory, List<Room>> categoryRoomMap,
        RoomRepository roomRepository)
    {
        WriteShort(ServerPacketId.NavigatorRooms);
        WriteString(tabName);
        WriteString(searchQuery);
        
        WriteInteger(categoryRoomMap.Count);

        foreach (var (category, rooms) in categoryRoomMap)
        {
            WriteString(category.CodeName);
            WriteString(category.Name);
            WriteInteger(0); // TODO: SEARCH ACTION?
            WriteBool(false); // TODO: is it collapsed?
            WriteInteger(0); // TODO: Show thumbnail? display mode

            WriteInteger(rooms.Count);
            
            foreach (var room in rooms)
            {
                var liveRoom = roomRepository.TryGetRoomById(room.Id);
                var userCount = liveRoom == null ? 0 : liveRoom.UserRepository.Count;
                
                WriteLong(room.Id);
                WriteString(room.Name);
                WriteLong(room.OwnerId);
                WriteString(room.Owner.Username);
                WriteInteger((int) room.Settings.AccessType);
                WriteInteger(userCount);
                WriteInteger(room.MaxUsersAllowed);
                WriteString(room.Description);
                WriteInteger(0); // unknown
                WriteInteger(room.PlayerLikes.Count);
                WriteInteger(0); // unknown
                WriteInteger(1); // TODO: category
                WriteInteger(room.Tags.Count);

                foreach (var tag in room.Tags)
                {
                    WriteString(tag.Name);
                }
                
                WriteInteger(0 | 8); // TODO: base
            }
        }
    }
}