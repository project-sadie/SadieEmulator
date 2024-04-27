using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Users;

public class RoomUserTagsWriter : AbstractPacketWriter
{
    public RoomUserTagsWriter(int userId, ICollection<PlayerTag> tags)
    {
        WriteShort(ServerPacketId.RoomUserTags);
        WriteInteger(userId);
        WriteInteger(tags.Count);

        foreach (var tag in tags)
        {
            WriteString(tag.Name);
        }
    }
}