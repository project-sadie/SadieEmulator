using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.API.DTOs.Players;
using Sadie.API.DTOs.Players.Furniture;
using Sadie.API.Interfaces.Game.Players;
using Sadie.Db.Models.Players;
using Sadie.Db.Models.Players.Furniture;
using Sadie.Game.Players;

namespace Sadie.Game.Mappers;

public class PlayerProfile : Profile
{
    public PlayerProfile(IServiceProvider provider)
    {
        CreateMap<PlayerDto, IPlayerLogic>()
            .ConstructUsing(x => new PlayerLogic(
                provider.GetRequiredService<ILogger<PlayerLogic>>(), x))
            .ForMember(x => x.NetworkObject, option => option.Ignore());
        
        CreateMap<PlayerSsoToken, PlayerSsoTokenDto>();
        CreateMap<Player, PlayerDto>();
        CreateMap<PlayerData, PlayerDataDto>().ReverseMap();
        CreateMap<PlayerAvatarData, PlayerAvatarDataDto>();
        
        CreateMap<PlayerNavigatorSettings, PlayerNavigatorSettingsDto>()
            .ForMember(dest => dest.Player, opt => opt.Ignore());
        
        CreateMap<PlayerGameSettings, PlayerGameSettingsDto>();
        CreateMap<PlayerRoomVisit, PlayerRoomVisitDto>().ReverseMap();
        
        CreateMap<PlayerTag, PlayerTagDto>();
        CreateMap<PlayerRoomLike, PlayerRoomLikeDto>();
        CreateMap<PlayerTag, PlayerTagDto>();
        CreateMap<PlayerRelationship, PlayerRelationshipDto>();
        CreateMap<PlayerBadge, PlayerBadgeDto>();
        CreateMap<PlayerFurnitureItemWiredData, PlayerFurnitureItemWiredDataDto>();
        CreateMap<PlayerWardrobeItem, PlayerWardrobeItemDto>();
        CreateMap<PlayerSubscription, PlayerSubscriptionDto>();
        CreateMap<PlayerRespect, PlayerRespectDto>();
        CreateMap<PlayerSavedSearch, PlayerSavedSearchDto>();
        CreateMap<PlayerFriendship, PlayerFriendshipDto>();
        CreateMap<PlayerBan, PlayerBanDto>();
        CreateMap<PlayerRoomBan, PlayerRoomBanDto>();
        CreateMap<PlayerIgnore, PlayerIgnoreDto>();
        CreateMap<PlayerBot, PlayerBotDto>();
    }
}