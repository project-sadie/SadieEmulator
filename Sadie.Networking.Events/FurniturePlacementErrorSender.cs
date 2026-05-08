using Sadie.API;
using Sadie.Core.Enums.Game.Rooms.Furniture;
using Sadie.Networking.Writers.Generic;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Core.Shared.Helpers;

namespace Sadie.Networking.Events;

public static class FurniturePlacementErrorSender
{
    public static async Task SendAsync(
        INetworkObject client,
        RoomFurniturePlacementError error)
    {
        await client.WriteToStreamAsync(new BubbleAlertWriter
        {
            Key = EnumHelpers.GetEnumDescription(
                NotificationType.FurniturePlacementError),
            Messages = new Dictionary<string, string>
            {
                { "message", error.ToString() }
            }
        });
    }
}