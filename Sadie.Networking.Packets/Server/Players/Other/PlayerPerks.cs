namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerPerks : NetworkPacketWriter
{
    public PlayerPerks() : base(ServerPacketId.PlayerPerks)
    {
        WriteInt(13);
        
        WriteString("USE_GUIDE_TOOL");
        WriteString("requirement.unfulfilled.helper_level_4");
        WriteBoolean(true);
        
        WriteString("GIVE_GUIDE_TOURS");
        WriteString("");
        WriteBoolean(true);
        
        WriteString("JUDGE_CHAT_REVIEWS");
        WriteString("requirement.unfulfilled.helper_level_6");
        WriteBoolean(true);
        
        WriteString("VOTE_IN_COMPETITIONS");
        WriteString("requirement.unfulfilled.helper_level_2");
        WriteBoolean(true);
        
        WriteString("CALL_ON_HELPERS");
        WriteString("");
        WriteBoolean(true);
        
        WriteString("CITIZEN");
        WriteString("");
        WriteBoolean(true);
        
        WriteString("TRADE");
        WriteString("requirement.unfulfilled.no_trade_lock");
        WriteBoolean(true);
        
        WriteString("HEIGHTMAP_EDITOR_BETA");
        WriteString("requirement.unfulfilled.feature_disabled");
        WriteBoolean(true);
        
        WriteString("BUILDER_AT_WORK");
        WriteString("");
        WriteBoolean(true);
        
        WriteString("CALL_ON_HELPERS");
        WriteString("");
        WriteBoolean(true);
        
        WriteString("CAMERA");
        WriteString("");
        WriteBoolean(true);
        
        WriteString("NAVIGATOR_PHASE_TWO_2014");
        WriteString("");
        WriteBoolean(true);
        
        WriteString("MOUSE_ZOOM");
        WriteString("");
        WriteBoolean(true);
        
        WriteString("NAVIGATOR_ROOM_THUMBNAIL_CAMERA");
        WriteString("");
        WriteBoolean(true);
        
        WriteString("HABBO_CLUB_OFFER_BETA");
        WriteString("");
        WriteBoolean(true);
    }
}