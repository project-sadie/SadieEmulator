using Sadie.API;
using Sadie.API.Game.Rooms;
using Sadie.API.Networking;
using Sadie.Database.Models.Navigator;
using Sadie.Database.Models.Rooms;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorRooms)]
public class NavigatorSearchResultPagesWriter : AbstractPacketWriter
{
    public required string? TabName { get; init; }
    public required string? SearchQuery { get; init; }
    public required Dictionary<NavigatorCategory, List<Room>> CategoryRoomMap { get; init; }
    public required IRoomRepository RoomRepository { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteString(TabName);
        writer.WriteString(SearchQuery);
        
        writer.WriteInteger(CategoryRoomMap.Count);

        foreach (var (category, rooms) in CategoryRoomMap)
        {
            writer.WriteString(category.CodeName);
            writer.WriteString(category.Name);
            writer.WriteInteger(0);
            writer.WriteBool(false);
            writer.WriteInteger(0);

            writer.WriteInteger(rooms.Count);
            
            foreach (var room in rooms)
            {
                var liveRoom = RoomRepository.TryGetRoomById(room.Id);
                var userCount = liveRoom == null ? 0 : liveRoom.UserRepository.Count;
                
                writer.WriteLong(room.Id);
                writer.WriteString(room.Name);
                writer.WriteLong(room.OwnerId);
                writer.WriteString(room.Owner.Username);
                writer.WriteInteger((int) room.Settings.AccessType);
                writer.WriteInteger(userCount);
                writer.WriteInteger(room.MaxUsersAllowed);
                writer.WriteString(room.Description);
                writer.WriteInteger(0);
                writer.WriteInteger(room.PlayerLikes.Count);
                writer.WriteInteger(0);
                writer.WriteInteger(1);
                writer.WriteInteger(room.Tags.Count);

                foreach (var tag in room.Tags)
                {
                    writer.WriteString(tag.Name);
                }
                
                writer.WriteInteger((int) RoomBitmask.ShowOwner);
            }
        }
    }
}