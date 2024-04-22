using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Game.Rooms.Packets.Writers;

public class RoomUserEffectWriter : NetworkPacketWriter
{
    public RoomUserEffectWriter(int userId, int effectId, TimeSpan delay = default)
    {
        WriteShort(ServerPacketId.RoomUserEffect);
        WriteInteger(userId);
        WriteInteger(effectId);
        WriteInteger(delay == default ? 0 : (int) delay.TotalMilliseconds);
    }
}