using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Wardrobe;
using Sadie.Networking.Packets;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Networking.Events.Handlers.Players.Wardrobe;

public class PlayerWardrobeSaveEventHandler(
    PlayerWardrobeSaveEventParser eventParser,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerWardrobeSave;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var player = client.Player;

        if (player == null)
        {
            return;
        }

        var wardrobeItem = new PlayerWardrobeItem
        {
            SlotId = eventParser.SlotId,
            FigureCode = eventParser.FigureCode,
            Gender = eventParser.Gender == "M" ? AvatarGender.Male : AvatarGender.Female
        };
            
        player.WardrobeItems.Add(wardrobeItem);
        await dbContext.SaveChangesAsync();
    }
}