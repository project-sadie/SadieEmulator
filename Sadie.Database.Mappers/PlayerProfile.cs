using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.API.Game.Players;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;

namespace Sadie.Database.Mappers;

public class PlayerProfile : Profile
{
    public PlayerProfile(IServiceProvider provider)
    {
        CreateMap<Player, IPlayerLogic>()
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