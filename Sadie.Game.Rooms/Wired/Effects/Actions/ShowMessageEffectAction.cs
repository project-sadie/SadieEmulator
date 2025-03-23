using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.API.Game.Rooms.Wired.Effects.Actions;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms.Packets.Writers.Users;

namespace Sadie.Game.Rooms.Wired.Effects.Actions;

public class ShowMessageEffectAction : IWiredEffectAction
{
    public string InteractionType => FurnitureItemInteractionType.WiredEffectShowMessage;

    public async Task ExecuteAsync(
        IRoomLogic room, 
        IRoomUser userWhoTriggered, 
        PlayerFurnitureItemPlacementData effect,
        IRoomWiredService wiredService)
    {
        if (effect.WiredData == null)
        {
            return;
        }
        
        await userWhoTriggered.NetworkObject.WriteToStreamAsync(new RoomUserWhisperWriter
        {
            SenderId = userWhoTriggered.Id,
            Message = effect.WiredData.Message ?? "",
            EmotionId = 0,
            ChatBubbleId = (int)ChatBubble.Alert,
            MessageLength = effect.WiredData.Message?.Length ?? 0,
            Urls = []
        });
    }
}