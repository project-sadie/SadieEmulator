using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.API.DTOs.Player;
using Sadie.API.Interfaces.Game.Players;
using Sadie.Db.Models.Players;
using Sadie.Game.Players;

namespace Sadie.Game.Mappers;

public class PlayerProfile : Profile
{
    public PlayerProfile(IServiceProvider provider)
    {
        CreateMap<PlayerDto, IPlayerLogic>()
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
        
        CreateMap<PlayerSsoToken, PlayerSsoTokenDto>();
        CreateMap<Player, PlayerDto>();
        CreateMap<PlayerData, PlayerDataDto>();
        CreateMap<PlayerAvatarData, PlayerAvatarDataDto>();
        CreateMap<PlayerNavigatorSettings, PlayerNavigatorSettingsDto>();
        CreateMap<PlayerGameSettings, PlayerGameSettingsDto>();
    }
}