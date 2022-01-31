namespace Sadie.Networking.Packets.Server.Handshake;

public class SendUniqueMachineIdPacket : NetworkPacketWriter
{
    public SendUniqueMachineIdPacket(string machineId) : base(ServerPacketIds.SendUniqueMachineId)
    {
        WriteString(machineId);
    }
}