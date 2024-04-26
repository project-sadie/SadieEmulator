using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Messenger;

public class PlayerMessageErrorWriter : AbstractPacketWriter
{
    public required int Error { get; init; }
    public required int TargetId { get; init; }
    public required string Unknown { get; init; }
}