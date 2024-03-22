using Sadie.Game.Players.Wardrobe;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Networking.Events.Players.Wardrobe;

public class PlayerWardrobeSaveEvent(IPlayerWardrobeDao wardrobeDao) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var slotId = reader.ReadInteger();
        var figureCode = reader.ReadString();
        var gender = reader.ReadString();

        var player = client.Player;

        if (player == null)
        {
            return;
        }

        var newRecord = !player
            .Data
            .WardrobeComponent
            .WardrobeItems
            .ContainsKey(slotId);
        
        var wardrobeItem = new PlayerWardrobeItem(
            slotId,
            figureCode, 
            gender == "M" ? AvatarGender.Male : AvatarGender.Female);

        await wardrobeDao.UpdateWardrobeItemAsync(player.Data.Id, wardrobeItem, newRecord);

        player.Data.WardrobeComponent.WardrobeItems[slotId] = wardrobeItem;
    }
}