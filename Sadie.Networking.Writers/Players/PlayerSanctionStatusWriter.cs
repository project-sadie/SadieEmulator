namespace Sadie.Networking.Packets.Server.Players;

public class PlayerSanctionStatusWriter : NetworkPacketWriter
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
        DateTime tradeLockedUntil) : base(ServerPacketId.PlayerSanctionStatus)
    {
        WriteBoolean(hasPreviousSanction);
        WriteBoolean(onProbation);
        WriteString(lastSanctionType);
        WriteInt(sanctionTime);
        WriteInt(unknown1);
        WriteString(reason);
        WriteString(probationStart.ToString());
        WriteInt(unknown2);
        WriteString(nextSanctionType);
        WriteInt(hoursForNextSanction);
        WriteInt(unknown3);
        WriteBoolean(muted);
        WriteString(tradeLockedUntil == DateTime.MinValue ? "" : tradeLockedUntil.ToString());
    }
}