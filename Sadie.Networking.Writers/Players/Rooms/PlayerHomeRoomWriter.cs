using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players.Rooms;

[PacketId(ServerPacketId.PlayerHomeRoom)]
public class PlayerHomeRoomWriter : AbstractPacketWriter
{
    public required int HomeRoom { get; init; }
    public required int RoomIdToEnter { get; init; }
}