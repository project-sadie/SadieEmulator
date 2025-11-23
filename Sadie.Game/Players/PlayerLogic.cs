using System.Text.RegularExpressions;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Sadie.API;
using Sadie.API.DTOs;
using Sadie.API.DTOs.Player;
using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.DTOs.Rooms;
using Sadie.API.DTOs.Server;
using Sadie.API.Interfaces.Game.Players;
using Sadie.Core.Enums.Game.Players;
using Sadie.Networking.Writers.Players;

namespace Sadie.Game.Players;

public class PlayerLogic : IPlayerLogic
{
    private readonly ILogger<PlayerLogic> _logger;
    private List<PlayerTagDto> _tags;

    public PlayerLogic(ILogger<PlayerLogic> logger,
        long id,
        string username,
        PlayerDataDto data)
    {
        _logger = logger;
        Id = id;
        Username = username;
        Data = data;
    }

    public IChannel? Channel { get; set; }
    public INetworkObject? NetworkObject { get; set; }
    public ICollection<PlayerBotDto> Bots { get; init; }
    public ICollection<PlayerRoomVisitDto> RoomVisits { get; init; }
    public IPlayerState State { get; } = new PlayerState();
    public bool Authenticated { get; set; }
    
    public int GetAcceptedFriendshipCount()
    {
        return IncomingFriendships.Count(x => x.Status == PlayerFriendshipStatus.Accepted) + 
               OutgoingFriendships.Count(x => x.Status == PlayerFriendshipStatus.Accepted);
    }

    public List<PlayerFriendshipDto> GetMergedFriendships()
    {
        return OutgoingFriendships
            .Concat(IncomingFriendships)
            .Where(x => x.Status == PlayerFriendshipStatus.Accepted)
            .ToList();
    }

    public bool IsFriendsWith(int targetId)
    {
        return IncomingFriendships.FirstOrDefault(x =>
                   x.OriginPlayerId == targetId && x.Status == PlayerFriendshipStatus.Accepted) !=
               null 
               ||
               OutgoingFriendships.FirstOrDefault(x =>
                   x.TargetPlayerId == targetId && x.Status == PlayerFriendshipStatus.Accepted) !=
               null;
    }

    public PlayerFriendshipDto? TryGetAcceptedFriendshipFor(long targetId)
    {
        var incoming = IncomingFriendships
            .FirstOrDefault(x => x.OriginPlayerId == targetId && x.Status == PlayerFriendshipStatus.Accepted);

        if (incoming != null)
        {
            return incoming;
        }
        
        return OutgoingFriendships
            .FirstOrDefault(x => x.OriginPlayerId == targetId && x.Status == PlayerFriendshipStatus.Accepted);
    }

    public PlayerFriendshipDto? TryGetFriendshipFor(long targetId)
    {
        var incoming = IncomingFriendships
            .FirstOrDefault(x => x.OriginPlayerId == targetId);

        if (incoming != null)
        {
            return incoming;
        }
        
        return OutgoingFriendships
            .FirstOrDefault(x => x.TargetPlayerId == targetId);
    }

    public void DeleteFriendshipFor(long targetId)
    {
        var incoming = IncomingFriendships
            .FirstOrDefault(x => x.OriginPlayerId == targetId);

        if (incoming != null)
        {
            IncomingFriendships.Remove(incoming);
        }

        var outgoing = OutgoingFriendships
            .FirstOrDefault(x => x.OriginPlayerId == targetId);
        
        if (outgoing != null)
        {
            OutgoingFriendships.Remove(outgoing);
        }
    }

    public bool HasPermission(string name)
    {
        return Roles.Any(r => r.Permissions.Any(x => x.Name == name));
    }

    public ValueTask DisposeAsync()
    {
        _logger.LogInformation($"Player '{Username}' has logged out");
        return ValueTask.CompletedTask;
    }

    public bool DeservesReward(string? rewardType, int intervalInSeconds)
    {
        var lastReward = RewardLogs
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault(x => x.Type == rewardType);

        return lastReward == null ||
               lastReward.CreatedAt < DateTime.Now.AddSeconds(-intervalInSeconds);
    }

    public async Task SendAlertAsync(string message)
    {
        await NetworkObject!.WriteToStreamAsync(new PlayerAlertWriter
        {
            Message = message
        });
    }
    
    public long Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public ICollection<RoleDto> Roles { get; init; } = [];
    public DateTimeOffset CreatedAt { get; init; }
    public PlayerDataDto? Data { get; set; }
    public PlayerAvatarDataDto? AvatarData { get; set; }

    List<PlayerTagDto> IPlayerLogic.Tags
    {
        get => _tags;
        init => _tags = value;
    }

    public ICollection<PlayerRoomLikeDto> RoomLikes { get; init; }
    public ICollection<PlayerTagDto> Tags { get; init; } = [];
    public ICollection<PlayerRelationshipDto> Relationships { get; init; } = [];
    public PlayerNavigatorSettingsDto? NavigatorSettings { get; set; }
    public ICollection<PlayerBadgeDto> Badges { get; init; } = [];
    public ICollection<PlayerFurnitureItemDto> FurnitureItems { get; init; }
    public ICollection<PlayerWardrobeItemDto> WardrobeItems { get; init; }
    public ICollection<PlayerSubscriptionDto> Subscriptions { get; init; } = [];
    public ICollection<PlayerRespectDto> Respects { get; init; }
    public ICollection<PlayerSavedSearchDto> SavedSearches { get; init; }
    public PlayerGameSettingsDto? GameSettings { get; set; }
    public ICollection<PlayerFriendshipDto> OutgoingFriendships { get; init; } = [];
    public ICollection<PlayerFriendshipDto> IncomingFriendships { get; init; } = [];
    public ICollection<ServerPeriodicCurrencyRewardLogDto> RewardLogs { get; init; }
    public ICollection<RoomDto> Rooms { get; set; }
    public ICollection<PlayerIgnoreDto> Ignores { get; set; }
    public ICollection<Group> Groups { get; init; }
    public ICollection<PlayerBanDto> Bans { get; init; } = [];
    public ICollection<PlayerSsoTokenDto> Tokens { get; init; } = [];
}