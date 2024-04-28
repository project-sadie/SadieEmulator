using Sadie.Networking.Serialization;

namespace Sadie.Networking.Writers.Players.Messenger;

public class PlayerMessageErrorWriter : AbstractPacketWriter
{
    public required int Error { get; init; }
    public required int TargetId { get; init; }
    public string Unknown { get; init; } = "";
}