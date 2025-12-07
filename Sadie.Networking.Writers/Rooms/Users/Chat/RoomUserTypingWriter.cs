using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users.Chat;

[PacketId(ServerPacketId.RoomUserTyping)]
public class RoomUserTypingWriter : AbstractPacketWriter
{
    public required long UserId { get; init; }
    public required bool IsTyping { get; init; }

    public override void OnConfigureRules()
    {
        Convert<int>(GetType().GetProperty(nameof(IsTyping))!, o => (bool)o ? 1 : 0);
    }
}