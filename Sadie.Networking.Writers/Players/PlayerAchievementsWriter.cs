using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players;

public class PlayerAchievementsWriter : AbstractPacketWriter
{
    public PlayerAchievementsWriter()
    {
        WriteShort(ServerPacketId.PlayerAchievements);
        WriteInteger(0);
        // TODO: foreach
        WriteString("");
    }
}