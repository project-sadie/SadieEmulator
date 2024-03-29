using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Rooms.Rights;

public class RoomGiveUserRightsParser
{
    public int PlayerId { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        PlayerId = reader.ReadInteger();
    }
}