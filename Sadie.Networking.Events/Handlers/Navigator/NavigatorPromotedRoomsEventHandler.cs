using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Rooms;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Packets.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

[PacketId(EventHandlerId.NavigatorPromotedRooms)]
public class NavigatorPromotedRoomsEventHandler(IPlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new NavigatorGuestRoomSearchResultWriter
        {
            SearchType = 2,
            SearchParam = "",
            Rooms =
            [
            ],
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
            },
            PlayerRepository = playerRepository
        });
    }
}