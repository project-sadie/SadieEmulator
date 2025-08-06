using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Competition;

namespace Sadie.Networking.Events.Handlers.Competition;

[PacketId(EventHandlerId.CompetitionTimingCode)]
public class CompetitionTimingCodeEventHandler : INetworkPacketEventHandler
{
    public string? Data { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (Data == null)
        {
            return;
        } 
        
        if (Data.Contains(';'))
        {
            var pieces = Data.Split(";");

            foreach (var piece in pieces)
            {
                if (piece.Contains(','))
                {
                    await client.WriteToStreamAsync(new CompetitionTimingCodeWriter
                    {
                        Schedule = piece,
                        Code = piece.Split(",").Last()
                    });
                }
                else
                {
                    await client.WriteToStreamAsync(new CompetitionTimingCodeWriter
                    {
                        Schedule = Data,
                        Code = piece
                    });
                }
            }
        }
        else
        {
            await client.WriteToStreamAsync(new CompetitionTimingCodeWriter
            {
                Schedule = Data,
                Code = Data.Split(",").Last()
            });
        }
    }
}