using Sadie.Game.Rooms;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserTagsWriter : NetworkPacketWriter
{
    public RoomUserTagsWriter(long roomUserId, List<string> tags)
    {
        WriteShort(ServerPacketId.RoomUserTags);
        WriteLong(roomUserId);
        WriteInt(tags.Count);

        foreach (var tag in tags)
        {
            WriteString(tag);
        }
    }
}