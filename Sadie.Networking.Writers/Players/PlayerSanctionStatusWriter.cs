using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerSanctionStatusWriter : AbstractPacketWriter
{
    public PlayerSanctionStatusWriter(
        bool hasPreviousSanction, 
        bool onProbation, 
        string lastSanctionType, 
        int sanctionTime, 
        int unknown1,
        string reason, 
        DateTime probationStart, 
        int unknown2,
        string nextSanctionType, 
        int hoursForNextSanction,
        int unknown3,
        bool muted, 
        DateTime tradeLockedUntil)
    {
        WriteShort(ServerPacketId.PlayerSanctionStatus);
        WriteBool(hasPreviousSanction);
        WriteBool(onProbation);
        WriteString(lastSanctionType);
        WriteInteger(sanctionTime);
        WriteInteger(unknown1);
        WriteString(reason);
        WriteString(probationStart.ToString());
        WriteInteger(unknown2);
        WriteString(nextSanctionType);
        WriteInteger(hoursForNextSanction);
        WriteInteger(unknown3);
        WriteBool(muted);
        WriteString(tradeLockedUntil == DateTime.MinValue ? "" : tradeLockedUntil.ToString());
    }
}