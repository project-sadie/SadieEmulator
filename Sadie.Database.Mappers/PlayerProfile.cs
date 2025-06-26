using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.API.Game.Players;
using Sadie.Db.Models.Players;
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
                Email = x.Email,
                Data = x.Data
            })
            .ForMember(x => x.NetworkObject, option => option.Ignore())
            .ForMember(x => x.Channel, option => option.Ignore());
    }
}