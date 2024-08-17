using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Players;

namespace Sadie.Game.Players;

public class PlayerLoader(SadieContext dbContext)
{
    public async Task<PlayerSsoToken?> GetTokenAsync(string token, int delayMs)
    {
        var tokenRecord = await dbContext
            .PlayerSsoToken
            .FirstOrDefaultAsync(x =>
                x.Token == token &&
                x.ExpiresAt > DateTime
                    .Now
                    .Subtract(TimeSpan.FromMilliseconds(delayMs)) &&
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