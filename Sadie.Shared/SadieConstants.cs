namespace Sadie.Shared;

public static class SadieConstants
{
    public static int HabboPacketMinLength => 4;
    public static int HabboPacketBufferSize => 4096;
    public static int HabboPacketMaxLength => HabboPacketBufferSize - 4;
    public static int HabboSsoMinLength => 20;

    public static string HabboPolicyXml => "<?xml version=\"1.0\"?>\r\n" +
                                           "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                                           "<cross-domain-policy>\r\n" +
                                           "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                                           "</cross-domain-policy>\x0";

    public static string DateTimeFormat => "yyyy-MM-dd HH:mm:ss";
    
    public static string MockHeightmap = "X000000000\r\nX000000000\r\nX000000000\r\nX000000000\r\nX000000000\r\nX000000000\r\nX000000000\r\nX000000000\r\n0000000000\r\nX000000000";

    public static List<string> NavigatorCategories = new()
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
        "eventcategory__Group Events",
    };
}