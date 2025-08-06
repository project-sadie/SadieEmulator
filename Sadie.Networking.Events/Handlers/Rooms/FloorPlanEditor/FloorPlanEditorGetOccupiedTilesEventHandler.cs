using Sadie.API.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Writers.Rooms.FloorPlanEditor;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.FloorPlanEditor;

[PacketId(EventHandlerId.FloorPlanEditorGetOccupiedTiles)]
public class FloorPlanEditorGetOccupiedTilesEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
        {
            return;
        }

        var blockedUserPoints = room
            .TileMap
            .UnitMap
            .Where(x => x.Value.Count > 0)
            .ToList()
            .Select(x => x.Key);

        await client.WriteToStreamAsync(new FloorPlanEditorOccupiedTilesWriter
        {
            Points = blockedUserPoints.ToList()
        });
    }
}