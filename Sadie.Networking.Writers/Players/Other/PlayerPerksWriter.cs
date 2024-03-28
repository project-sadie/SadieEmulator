using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerPerksWriter : NetworkPacketWriter
{
    public PlayerPerksWriter()
    {
        WriteShort(ServerPacketId.PlayerPerks);
        WriteInteger(13);
        
        WriteString("USE_GUIDE_TOOL");
        WriteString("requirement.unfulfilled.helper_level_4");
        WriteBool(true);
        
        WriteString("GIVE_GUIDE_TOURS");
        WriteString("");
        WriteBool(true);
        
        WriteString("JUDGE_CHAT_REVIEWS");
        WriteString("requirement.unfulfilled.helper_level_6");
        WriteBool(true);
        
        WriteString("VOTE_IN_COMPETITIONS");
        WriteString("requirement.unfulfilled.helper_level_2");
        WriteBool(true);
        
        WriteString("CALL_ON_HELPERS");
        WriteString("");
        WriteBool(true);
        
        WriteString("CITIZEN");
        WriteString("");
        WriteBool(true);
        
        WriteString("TRADE");
        WriteString("requirement.unfulfilled.no_trade_lock");
        WriteBool(true);
        
        WriteString("HEIGHTMAP_EDITOR_BETA");
        WriteString("requirement.unfulfilled.feature_disabled");
        WriteBool(true);
        
        WriteString("BUILDER_AT_WORK");
        WriteString("");
        WriteBool(true);
        
        WriteString("CALL_ON_HELPERS");
        WriteString("");
        WriteBool(true);
        
        WriteString("CAMERA");
        WriteString("");
        WriteBool(true);
        
        WriteString("NAVIGATOR_PHASE_TWO_2014");
        WriteString("");
        WriteBool(true);
        
        WriteString("MOUSE_ZOOM");
        WriteString("");
        WriteBool(true);
        
        WriteString("NAVIGATOR_ROOM_THUMBNAIL_CAMERA");
        WriteString("");
        WriteBool(true);
        
        WriteString("HABBO_CLUB_OFFER_BETA");
        WriteString("");
        WriteBool(true);
    }
}