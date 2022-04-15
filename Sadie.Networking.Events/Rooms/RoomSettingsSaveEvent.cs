using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Rooms;

public class RoomSettingsSaveEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomSettingsSaveEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        
    }
}