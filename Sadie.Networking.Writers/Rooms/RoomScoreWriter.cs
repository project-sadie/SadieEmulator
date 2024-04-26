using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomScoreWriter : AbstractPacketWriter
{
    public RoomScoreWriter(int score, bool canUpvote)
    {
        WriteShort(ServerPacketId.RoomScore);
        WriteInteger(score);
        WriteBool(canUpvote);
    }
}