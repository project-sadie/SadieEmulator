using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Players;

namespace Sadie.Game.Players;

public class PlayerLoader(SadieContext dbContext)
{
    public async Task<PlayerSsoToken?> GetTokenAsync(string token, int delayMs)
    {
        var expires = DateTime
            .Now
            .Subtract(TimeSpan.FromMilliseconds(delayMs));
        
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
        
        tokenRecord.UsedAt = DateTime.Now;

        dbContext.Entry(tokenRecord).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();

        return tokenRecord;
    }
}