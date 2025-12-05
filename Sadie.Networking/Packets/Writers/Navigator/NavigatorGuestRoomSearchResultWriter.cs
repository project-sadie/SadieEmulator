using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Enums.Game.Rooms;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorPromotedRooms)]
public class NavigatorGuestRoomSearchResultWriter : AbstractPacketWriter
{
    public required int SearchType { get; init; }
    public required string SearchParam { get; init; }
    public required List<RoomData> Rooms { get; init; }
    public required bool HasAdditional { get; init; }
    public required OfficialRoomEntryData OfficialRoomEntryData { get; init; }
    public required IPlayerRepository PlayerRepository { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(OfficialRoomEntryData))!, async writer =>
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
                    writer.WriteString((await PlayerRepository.GetPlayerByIdAsyncT(guestRoom.OwnerId))?.Username ?? "Unknown User");
                    writer.WriteInteger((int) guestRoom.Settings.AccessType);
                    writer.WriteInteger(OfficialRoomEntryData.UserCount);
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