using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerMeMenuSettings)]
public class PlayerMeMenuSettingsWriter : AbstractPacketWriter
{
    public required int SystemVolume { get; init; }
    public required int FurnitureVolume { get; init; }
    public required int TraxVolume { get; init; }
    public required bool OldChat { get; init; }
    public required bool BlockRoomInvites { get; init; }
    public required bool BlockCameraFollow { get; init; }
    public required int UiFlags { get; init; }
    public required int ChatBubble { get; init; }
}