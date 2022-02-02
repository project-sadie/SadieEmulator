namespace Sadie.Networking.Packets.Server.Handshake;

public class SendUniqueMachineIdPacket : NetworkPacketWriter
{
    public SendUniqueMachineIdPacket(string machineId) : base(ServerPacketId.SendUniqueMachineId)
    {
        WriteString(machineId);
    }
}