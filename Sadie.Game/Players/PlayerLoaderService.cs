using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sadie.API.DTOs.Player;
using Sadie.API.Interfaces.Game.Players;
using Sadie.Db;
using Sadie.Game.Players.Options;

namespace Sadie.Game.Players;

public class PlayerLoaderService(IDbContextFactory<SadieDbContext> dbContextFactory,
    IOptions<PlayerOptions> playerOptions,
    IMapper mapper) : IPlayerLoaderService
{
    public async Task<PlayerSsoTokenDto?> GetTokenAsync(string token, int delayMs)
    {
        var expires = DateTime.Now
            .Subtract(TimeSpan.FromMilliseconds(delayMs));

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var entity = await dbContext.PlayerSsoToken
            .FirstOrDefaultAsync(x =>
                x.Token == token &&
                x.ExpiresAt >= expires &&
                x.UsedAt == null);

        if (entity == null)
        {
            return null;
        }

        if (playerOptions.Value.CanReuseSsoTokens)
        {
            return mapper.Map<PlayerSsoTokenDto>(entity);
        }

        entity.UsedAt = DateTime.Now;
        await dbContext.SaveChangesAsync();

        return mapper.Map<PlayerSsoTokenDto>(entity);
    }
}