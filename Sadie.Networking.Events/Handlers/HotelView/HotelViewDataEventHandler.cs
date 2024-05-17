using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.HotelView;

namespace Sadie.Networking.Events.Handlers.HotelView;

[PacketId(EventHandlerIds.HotelViewData)]
public class HotelViewDataEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var unknown1 = eventParser.Unknown1;
        
        if (unknown1.Contains(';'))
        {
            var pieces = unknown1.Split(";");

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
                        Key = unknown1,
                        Value = piece
                    });
                }
            }
        }
        else
        {
            await client.WriteToStreamAsync(new HotelViewDataWriter
            {
                Key = unknown1,
                Value = unknown1.Split(",").Last()
            });
        }
    }
}