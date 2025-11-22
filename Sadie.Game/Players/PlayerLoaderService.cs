using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sadie.API.DTOs.Player;
using Sadie.API.Interfaces.Game.Players;
using Sadie.Db;
using Sadie.Game.Players.Options;

namespace Sadie.Game.Players;

public class PlayerLoaderService(IDbContextFactory<SadieDbContext> dbContextFactory,
    IOptions<PlayerOptions> playerOptions) : IPlayerLoaderService
{
    public async Task<PlayerSsoTokenDto?> GetTokenAsync(string token, int delayMs)
    {
        var expires = DateTime
            .Now
            .Subtract(TimeSpan.FromMilliseconds(delayMs));

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var tokenRecord = await dbContext
            .PlayerSsoToken
            .FirstOrDefaultAsync(x =>
                x.Token == token &&
                x.ExpiresAt > expires &&
                x.UsedAt == null);

        if (tokenRecord == null)
        {
            return tokenRecord;
        }

        if (playerOptions.Value.CanReuseSsoTokens)
        {
            return tokenRecord;
        }
        
        tokenRecord.UsedAt = DateTime.Now;

        dbContext.Entry(tokenRecord).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();

        return tokenRecord;
    }
}