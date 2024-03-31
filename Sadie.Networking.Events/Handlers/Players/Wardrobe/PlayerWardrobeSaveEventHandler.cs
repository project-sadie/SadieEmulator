using Sadie.Game.Players.Wardrobe;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Wardrobe;
using Sadie.Networking.Packets;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Networking.Events.Handlers.Players.Wardrobe;

public class PlayerWardrobeSaveEventHandler(
    PlayerWardrobeSaveEventParser eventParser,
    IPlayerWardrobeDao wardrobeDao) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var player = client.Player;

        if (player == null)
        {
            return;
        }

        var newRecord = !player
            .Data
            .WardrobeComponent
            .WardrobeItems
            .ContainsKey(eventParser.SlotId);
        
        var wardrobeItem = new PlayerWardrobeItem(
            eventParser.SlotId,
            eventParser.FigureCode, 
            eventParser.Gender == "M" ? AvatarGender.Male : AvatarGender.Female);

        await wardrobeDao.UpdateWardrobeItemAsync(player.Data.Id, wardrobeItem, newRecord);

        player.Data.WardrobeComponent.WardrobeItems[eventParser.SlotId] = wardrobeItem;
    }
}