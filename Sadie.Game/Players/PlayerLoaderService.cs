using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sadie.API.Game.Players;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Options.Options;

namespace Sadie.Game.Players;

public class PlayerLoaderService(IDbContextFactory<SadieContext> dbContextFactory,
    IOptions<PlayerOptions> playerOptions) : IPlayerLoaderService
{
    public async Task<PlayerSsoToken?> GetTokenAsync(string token, int delayMs)
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