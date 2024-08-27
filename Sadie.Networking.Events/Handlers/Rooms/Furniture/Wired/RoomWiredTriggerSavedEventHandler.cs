using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture.Wired;

[PacketId(EventHandlerId.RoomWiredTriggerSaved)]
public class RoomWiredTriggerSavedEventHandler : INetworkPacketEventHandler
{
    public required int ItemId { get; set; }
    public required List<int> Parameters { get; set; }
    public required string Input { get; set; }
    public required List<int> ItemIds { get; set; }
    public required int SelectionCode { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.RoomUser == null || 
            !client.RoomUser.HasRights())
        {
            return;
        }

        var room = client.RoomUser?.Room;
        var playerItem = room?.FurnitureItems.FirstOrDefault(x => x.Id == ItemId);

        if (playerItem == null)
        {
            return;
        }
        
        await client.WriteToStreamAsync(new WiredSavedWriter());
    }
}