using Sadie.Enums.Unsorted;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

[PacketId(EventHandlerId.NavigatorPromotedRooms)]
public class NavigatorPromotedRoomsEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new NavigatorGuestRoomSearchResultWriter
        {
            SearchType = 2,
            SearchParam = "",
            Rooms = [],
            HasAdditional = true,
            OfficialRoomEntryData = new OfficialRoomEntryData
            {
                Index = 0,
                PopupCaption = "A",
                PopupDescription = "B",
                ShowDetails = 1,
                PictureText = "C",
                PictureRef = "D",
                FolderId = 1,
                UserCount = 1,
                Type = OfficialRoomEntryDataType.Tag,
                Unknown14 = "E"
            }
        });
    }
}