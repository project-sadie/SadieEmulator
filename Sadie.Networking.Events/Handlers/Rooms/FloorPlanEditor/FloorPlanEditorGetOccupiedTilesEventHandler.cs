using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.FloorPlanEditor;

namespace Sadie.Networking.Events.Handlers.Rooms.FloorPlanEditor;

[PacketId(EventHandlerIds.FloorPlanEditorGetOccupiedTiles)]
public class FloorPlanEditorGetOccupiedTilesEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        var blockedUserPoints = room
            .TileMap
            .UserMap
            .Where(x => x.Value.Count > 0)
            .ToList()
            .Select(x => x.Key);

        var blockedBotPoints = room
            .TileMap
            .BotMap
            .Where(x => x.Value.Count > 0)
            .ToList()
            .Select(x => x.Key);

        await client.WriteToStreamAsync(new FloorPlanEditorOccupiedTilesWriter
        {
            Points = blockedUserPoints.Concat(blockedBotPoints).ToList()
        });
    }
}