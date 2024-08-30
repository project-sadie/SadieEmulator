using Sadie.API;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Dtos;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerData)]
public class PlayerDataEventHandler() : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null)
        {
            return;
        }
        
        await client.WriteToStreamAsync(new PlayerDataWriter
        {
            Player = client.Player
        });

        var perks = new List<IPerkData>
        {
            new PerkData("USE_GUIDE_TOOL", "requirement.unfulfilled.helper_level_4", true),
            new PerkData("GIVE_GUIDE_TOURS", "", true),
            new PerkData("JUDGE_CHAT_REVIEWS", "requirement.unfulfilled.helper_level_6", true),
            new PerkData("VOTE_IN_COMPETITIONS", "requirement.unfulfilled.helper_level_2", true),
            new PerkData("CALL_ON_HELPERS", "", true),
            new PerkData("CITIZEN", "", true),
            new PerkData("TRADE", "requirement.unfulfilled.no_trade_lock", true),
            new PerkData("HEIGHTMAP_EDITOR_BETA", "requirement.unfulfilled.feature_disabled", true),
            new PerkData("BUILDER_AT_WORK", "", true),
            new PerkData("CALL_ON_HELPERS", "", true),
            new PerkData("CAMERA", "", true),
            new PerkData("NAVIGATOR_PHASE_TWO_2014", "", true),
            new PerkData("MOUSE_ZOOM", "", true),
            new PerkData("NAVIGATOR_ROOM_THUMBNAIL_CAMERA", "", true),
            new PerkData("HABBO_CLUB_OFFER_BETA", "", true),
        };
        
        await client.WriteToStreamAsync(new PlayerPerksWriter
        {
            Perks = perks
        });
    }
}