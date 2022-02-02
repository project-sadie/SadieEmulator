namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorCollapsedCategoriesWriter : NetworkPacketWriter
{
    public NavigatorCollapsedCategoriesWriter() : base(ServerPacketId.NewNavigatorCollapsedCategories)
    {
        WriteInt(46);
        WriteString("new_ads");
        WriteString("friend_finding");
        WriteString("staffpicks");
        WriteString("with_friends");
        WriteString("with_rights");
        WriteString("query");
        WriteString("recommended");
        WriteString("my_groups");
        WriteString("favorites");
        WriteString("history");
        WriteString("top_promotions");
        WriteString("campaign_target");
        WriteString("friends_rooms");
        WriteString("groups");
        WriteString("metadata");
        WriteString("history_freq");
        WriteString("highest_score");
        WriteString("competition");
        WriteString("category__Agencies");
        WriteString("category__Role Playing");
        WriteString("category__Global Chat & Discussi");
        WriteString("category__GLOBAL BUILDING AND DE");
        WriteString("category__global party");
        WriteString("category__global games");
        WriteString("category__global fansite");
        WriteString("category__global help");
        WriteString("category__Trading");
        WriteString("category__global personal space");
        WriteString("category__Habbo Life");
        WriteString("category__TRADING");
        WriteString("category__global official");
        WriteString("category__global trade");
        WriteString("category__global reviews");
        WriteString("category__global bc");
        WriteString("category__global personal space");
        WriteString("eventcategory__Hottest Events");
        WriteString("eventcategory__Parties & Music");
        WriteString("eventcategory__Role Play");
        WriteString("eventcategory__Help Desk");
        WriteString("eventcategory__Trading");
        WriteString("eventcategory__Games");
        WriteString("eventcategory__Debates & Discuss");
        WriteString("eventcategory__Grand Openings");
        WriteString("eventcategory__Friending");
        WriteString("eventcategory__Jobs");
        WriteString("eventcategory__Group Events");
    }
}