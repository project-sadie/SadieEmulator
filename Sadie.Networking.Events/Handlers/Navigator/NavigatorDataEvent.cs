using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;
using Sadie.Networking.Writers.Players.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

public class NavigatorDataEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var metaData = new Dictionary<string, int>
        {
            {"official_view", 0},
            {"hotel_view", 0},
            {"roomads_view", 0},
            {"myworld_view", 0}
        };
        
        var categories = new List<string>
        {
            "new_ads",
            "friend_finding",
            "staffpicks",
            "with_friends",
            "with_rights",
            "query",
            "recommended",
            "my_groups",
            "favorites",
            "history",
            "top_promotions",
            "campaign_target",
            "friends_rooms",
            "groups",
            "metadata",
            "history_freq",
            "highest_score",
            "competition",
            "category__Agencies",
            "category__Role Playing",
            "category__Global Chat & Discussi",
            "category__GLOBAL BUILDING AND DE",
            "category__global party",
            "category__global games",
            "category__global fansite",
            "category__global help",
            "category__Trading",
            "category__global personal space",
            "category__Habbo Life",
            "category__TRADING",
            "category__global official",
            "category__global trade",
            "category__global bc",
            "category__global personal space",
            "eventcategory__Hottest Events",
            "eventcategory__Parties & Music",
            "eventcategory__Role Play",
            "eventcategory__Help Desk",
            "eventcategory__Trading",
            "eventcategory__Games",
            "eventcategory__Debates & Discuss",
            "eventcategory__Grand Openings",
            "eventcategory__Friending",
            "eventcategory__Jobs",
            "eventcategory__Group Events"
        };

        var savedSearches = client.Player.Data.SavedSearches;
        
        await client.WriteToStreamAsync(new NavigatorMetaDataWriter(metaData).GetAllBytes());
        await client.WriteToStreamAsync(new NavigatorLiftedRoomsWriter(new List<IRoom>()).GetAllBytes());
        await client.WriteToStreamAsync(new NavigatorCollapsedCategoriesWriter(categories).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerSavedSearchesWriter(savedSearches).GetAllBytes());
    }
}