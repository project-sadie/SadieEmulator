using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerAchievementsWriter : NetworkPacketWriter
{
    public PlayerAchievementsWriter()
    {
        WriteShort(ServerPacketId.PlayerAchievements);
        WriteInteger(0);
        // TODO: foreach
        WriteString("");
    }
}