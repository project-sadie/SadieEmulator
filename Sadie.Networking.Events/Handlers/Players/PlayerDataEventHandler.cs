using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerDataEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerData;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerDataWriter
        {
            Player = client.Player
        });

        var perks = new List<PlayerPerk>
        {
            new("USE_GUIDE_TOOL", "requirement.unfulfilled.helper_level_4", true),
            new("GIVE_GUIDE_TOURS", "", true),
            new("JUDGE_CHAT_REVIEWS", "requirement.unfulfilled.helper_level_6", true),
            new("VOTE_IN_COMPETITIONS", "requirement.unfulfilled.helper_level_2", true),
            new("CALL_ON_HELPERS", "", true),
            new("CITIZEN", "", true),
            new("TRADE", "requirement.unfulfilled.no_trade_lock", true),
            new("HEIGHTMAP_EDITOR_BETA", "requirement.unfulfilled.feature_disabled", true),
            new("BUILDER_AT_WORK", "", true),
            new("CALL_ON_HELPERS", "", true),
            new("CAMERA", "", true),
            new("NAVIGATOR_PHASE_TWO_2014", "", true),
            new("MOUSE_ZOOM", "", true),
            new("NAVIGATOR_ROOM_THUMBNAIL_CAMERA", "", true),
            new("HABBO_CLUB_OFFER_BETA", "", true),
        };
        
        await client.WriteToStreamAsync(new PlayerPerksWriter
        {
            Perks = perks
        });
    }
}