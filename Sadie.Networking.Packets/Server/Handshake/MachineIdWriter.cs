namespace Sadie.Networking.Packets.Server.Handshake;

internal class MachineIdWriter : NetworkPacketWriter
{
    internal MachineIdWriter(string machineId) : base(ServerPacketId.UniqueMachineId)
    {
        WriteString(machineId);
    }
}