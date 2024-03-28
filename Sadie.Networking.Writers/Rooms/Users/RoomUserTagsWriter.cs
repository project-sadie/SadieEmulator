using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserTagsWriter : NetworkPacketWriter
{
    public RoomUserTagsWriter(int userId, List<string> tags)
    {
        WriteShort(ServerPacketId.RoomUserTags);
        WriteInteger(userId);
        WriteInteger(tags.Count);

        foreach (var tag in tags)
        {
            WriteString(tag);
        }
    }
}