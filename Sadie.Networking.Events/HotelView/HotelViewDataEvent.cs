using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.HotelView;

namespace Sadie.Networking.Events.HotelView;

public class HotelViewDataEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var unknown1 = reader.ReadString();
        
        if (unknown1.Contains(';'))
        {
            var pieces = unknown1.Split(";");

            foreach (var piece in pieces)
            {
                if (piece.Contains(','))
                {
                    await client.WriteToStreamAsync(new HotelViewDataWriter(piece, piece.Split(",").Last()).GetAllBytes());
                }
                else
                {
                    await client.WriteToStreamAsync(new HotelViewDataWriter(unknown1, piece).GetAllBytes());
                }
            }
        }
        else
        {
            await client.WriteToStreamAsync(new HotelViewDataWriter(unknown1, unknown1.Split(",").Last()).GetAllBytes());
        }
    }
}