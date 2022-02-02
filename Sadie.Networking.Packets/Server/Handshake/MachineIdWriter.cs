namespace Sadie.Networking.Packets.Server.Handshake;

public class MachineIdWriter : NetworkPacketWriter
{
    public MachineIdWriter(string machineId) : base(ServerPacketId.SendUniqueMachineId)
    {
        WriteString(machineId);
    }
}