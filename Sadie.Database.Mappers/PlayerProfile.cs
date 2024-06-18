using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
#pragma warning disable CS8604 // Possible null reference argument.

namespace Sadie.Database.Mappers;

public class PlayerProfile : Profile
{
    public PlayerProfile(IServiceProvider provider)
    {
        CreateMap<Player, PlayerLogic>()
            .ConstructUsing(x => new PlayerLogic(
                provider.GetRequiredService<ILogger<PlayerLogic>>(),
                x.Id,
                x.Username,
                x.Data)
            {
                Username = x.Username,
                Email = x.Email
            })
            .ForMember(x => x.NetworkObject, option => option.Ignore())
            .ForMember(x => x.Channel, option => option.Ignore());
    }
}