using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomChatSettingsWriter : AbstractPacketWriter
{
    public RoomChatSettingsWriter(int chatType, int chatWeight, int chatSpeed, int chatDistance, int chatProtection)
    {
        WriteShort(ServerPacketId.RoomChatSettings);
        WriteInteger(chatType);
        WriteInteger(chatWeight);
        WriteInteger(chatSpeed);
        WriteInteger(chatDistance);
        WriteInteger(chatProtection);
    }
}