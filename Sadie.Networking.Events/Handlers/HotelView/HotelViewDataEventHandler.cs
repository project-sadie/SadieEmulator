using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.HotelView;

namespace Sadie.Networking.Events.Handlers.HotelView;

[PacketId(EventHandlerId.HotelViewData)]
public class HotelViewDataEventHandler : INetworkPacketEventHandler
{
    public string? Unknown1 { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (Unknown1.Contains(';'))
        {
            var pieces = Unknown1.Split(";");

            foreach (var piece in pieces)
            {
                if (piece.Contains(','))
                {
                    await client.WriteToStreamAsync(new HotelViewDataWriter
                    {
                        Key = piece,
                        Value = piece.Split(",").Last()
                    });
                }
                else
                {
                    await client.WriteToStreamAsync(new HotelViewDataWriter
                    {
                        Key = Unknown1,
                        Value = piece
                    });
                }
            }
        }
        else
        {
            await client.WriteToStreamAsync(new HotelViewDataWriter
            {
                Key = Unknown1,
                Value = Unknown1.Split(",").Last()
            });
        }
    }
}