using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.FloorPlanEditor;

namespace Sadie.Networking.Events.Handlers.Rooms.FloorPlanEditor;

[PacketId(EventHandlerIds.FloorPlanEditorGetOccupiedTiles)]
public class FloorPlanEditorGetOccupiedTilesEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        var blockedUserTiles = room
            .TileMap
            .UserMap
            .Where(x => x.Value.Count > 0)
            .Select(x => x.Key)
            .ToDictionary(k => k.X, v => v.Y);

        var blockedBotTiles = room
            .TileMap
            .BotMap
            .Where(x => x.Value.Count > 0)
            .Select(x => x.Key)
            .ToDictionary(k => k.X, v => v.Y);

        blockedUserTiles
            .ToList()
            .ForEach(x => blockedBotTiles.Add(x.Key, x.Value));

        await client.WriteToStreamAsync(new FloorPlanEditorOccupiedTilesWriter
        {
            Tiles = blockedBotTiles
        });
    }
}