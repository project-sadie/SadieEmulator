using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.HotelView;

namespace Sadie.Networking.Packets.Client.HotelView;

public class RequestHotelViewDataEvent : INetworkPacketEvent
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