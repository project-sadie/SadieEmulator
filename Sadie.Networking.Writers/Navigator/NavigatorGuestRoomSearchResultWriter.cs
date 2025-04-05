using Sadie.API.Networking;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorPromotedRooms)]
public class NavigatorGuestRoomSearchResultWriter : AbstractPacketWriter
{
    public required int SearchType { get; init; }
    public required string SearchParam { get; init; }
    public required List<RoomData> Rooms { get; init; }
    public required bool HasAdditional { get; init; }
    public required OfficialRoomEntryData OfficialRoomEntryData { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(OfficialRoomEntryData))!, writer =>
        {
            if (!HasAdditional)
            {
                return;
            }
            
            writer.WriteInteger(OfficialRoomEntryData.Index);
            writer.WriteString(OfficialRoomEntryData.PopupCaption);
            writer.WriteString(OfficialRoomEntryData.PopupDescription);
            writer.WriteInteger(OfficialRoomEntryData.ShowDetails);
            writer.WriteString(OfficialRoomEntryData.PictureText);
            writer.WriteString(OfficialRoomEntryData.PictureRef);
            
            writer.WriteInteger(OfficialRoomEntryData.FolderId);
            writer.WriteInteger(OfficialRoomEntryData.UserCount);
            writer.WriteInteger((int) OfficialRoomEntryData.Type);

            switch (OfficialRoomEntryData.Type)
            {
                case OfficialRoomEntryDataType.Tag:
                    writer.WriteString(OfficialRoomEntryData.Tag ?? "");
                    break;
                case OfficialRoomEntryDataType.GuestRoom:
                {
                    var guestRoom = OfficialRoomEntryData.GuestRoom;
                    
                    writer.WriteInteger(guestRoom!.Id);
                    writer.WriteString(guestRoom.Name);
                    writer.WriteLong(guestRoom.OwnerId);
                    writer.WriteString(guestRoom.Owner!.Username);
                    writer.WriteInteger((int) guestRoom.Settings!.AccessType);
                    writer.WriteInteger(guestRoom.UserRepository.Count);
                    writer.WriteInteger(guestRoom.MaxUsersAllowed);
                    writer.WriteString(guestRoom.Description);
                    writer.WriteInteger((int) guestRoom.Settings.TradeOption);
                    writer.WriteInteger(guestRoom.PlayerLikes.Count);
                    writer.WriteInteger(0);
                    writer.WriteInteger(1);
                    writer.WriteInteger(guestRoom.Tags.Count);

                    foreach (var tag in guestRoom.Tags)
                    {
                        writer.WriteString(tag.Name);
                    }
                
                    writer.WriteInteger((int) RoomBitmask.ShowOwner);
                    break;
                }
                case OfficialRoomEntryDataType.Folder:
                    break;
                default:
                    writer.WriteBool(OfficialRoomEntryData.Open);
                    break;
            }
        });
    }
}