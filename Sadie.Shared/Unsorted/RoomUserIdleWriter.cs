using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Shared.Unsorted;

public class RoomUserIdleWriter : AbstractPacketWriter
{
    public required int UserId { get; set; }
    public required bool IsIdle { get; set; }
}