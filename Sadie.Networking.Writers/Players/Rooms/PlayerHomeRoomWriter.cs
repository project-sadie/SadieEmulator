using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players.Rooms;

[PacketId(ServerPacketId.PlayerHomeRoom)]
public class PlayerHomeRoomWriter : AbstractPacketWriter
{
    public required int HomeRoom { get; init; }
    public required int RoomIdToEnter { get; init; }
}