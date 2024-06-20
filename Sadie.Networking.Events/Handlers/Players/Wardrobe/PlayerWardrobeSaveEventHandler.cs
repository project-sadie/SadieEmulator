using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Networking.Events.Handlers.Players.Wardrobe;

[PacketId(EventHandlerIds.PlayerWardrobeSave)]
public class PlayerWardrobeSaveEventHandler(
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public int SlotId { get; set; }
    public required string FigureCode { get; set; }
    public required string Gender { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var player = client.Player;

        if (player == null)
        {
            return;
        }

        var wardrobeItem = new PlayerWardrobeItem
        {
            SlotId = SlotId,
            FigureCode = FigureCode,
            Gender = Gender == "M" ? AvatarGender.Male : AvatarGender.Female
        };
            
        player.WardrobeItems.Add(wardrobeItem);
        await dbContext.SaveChangesAsync();
    }
}