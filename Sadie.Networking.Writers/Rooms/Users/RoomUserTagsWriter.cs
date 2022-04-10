using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserTagsWriter : NetworkPacketWriter
{
    public RoomUserTagsWriter(int userId, List<string> tags)
    {
        WriteShort(ServerPacketId.RoomUserTags);
        WriteInt(userId);
        WriteInt(tags.Count);

        foreach (var tag in tags)
        {
            WriteString(tag);
        }
    }
}