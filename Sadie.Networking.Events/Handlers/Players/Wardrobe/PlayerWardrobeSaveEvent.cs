using Sadie.Game.Players.Wardrobe;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Wardrobe;
using Sadie.Networking.Packets;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Networking.Events.Handlers.Players.Wardrobe;

public class PlayerWardrobeSaveEvent(
    PlayerWardrobeSaveParser parser,
    IPlayerWardrobeDao wardrobeDao) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        var player = client.Player;

        if (player == null)
        {
            return;
        }

        var newRecord = !player
            .Data
            .WardrobeComponent
            .WardrobeItems
            .ContainsKey(parser.SlotId);
        
        var wardrobeItem = new PlayerWardrobeItem(
            parser.SlotId,
            parser.FigureCode, 
            parser.Gender == "M" ? AvatarGender.Male : AvatarGender.Female);

        await wardrobeDao.UpdateWardrobeItemAsync(player.Data.Id, wardrobeItem, newRecord);

        player.Data.WardrobeComponent.WardrobeItems[parser.SlotId] = wardrobeItem;
    }
}