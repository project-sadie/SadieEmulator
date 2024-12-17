using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.Database;
using Sadie.Database.Models.Players;

namespace Sadie.Game.Players;

public class PlayerService(
    IDbContextFactory<SadieContext> dbContextFactory, 
    IPlayerRepository playerRepository,
    IMapper mapper) : IPlayerService
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
        
        tokenRecord.UsedAt = DateTime.Now;

        dbContext.Entry(tokenRecord).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();

        return tokenRecord;
    }

    public async Task<Player?> GetPlayerByIdAsync(int id, bool checkInMemory = true)
    {
        if (checkInMemory)
        {
            var online = playerRepository.GetPlayerLogicById(id);
        
            if (online != null)
            {
                return mapper.Map<Player>(online);
            }
        }
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        return await dbContext
            .Set<Player>()
            .Include(x => x.Data)
            .Include(x => x.GameSettings)
            .Include(x => x.NavigatorSettings)
            .Include(x => x.AvatarData)
            .Include(x => x.Relationships).ThenInclude(x => x.TargetPlayer)
            .Include(x => x.Bans)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<PlayerRelationship>> GetRelationshipsForPlayerAsync(int playerId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        return await dbContext
            .Set<PlayerRelationship>()
            .Where(x => x.OriginPlayerId == playerId || x.TargetPlayerId == playerId)
            .ToListAsync();
    }

    public async Task<List<Player>> GetPlayersForSearchAsync(string searchQuery, int[] excludeIds)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        return await dbContext
            .Set<Player>()
            .Include(x => x.AvatarData)
            .Where(x => 
                x.Username.Contains(searchQuery) && 
                !excludeIds.Contains(x.Id))
            .ToListAsync();
    }
    
    public async Task<Player?> GetPlayerByUsernameAsync(string username, bool checkInMemory = true)
    {
        if (checkInMemory)
        {
            var online = playerRepository.GetPlayerLogicByUsername(username);

            if (online != null)
            {
                return mapper.Map<Player>(online);
            }
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        return await dbContext
            .Set<Player>()
            .Include(x => x.Data)
            .FirstOrDefaultAsync(x => x.Username == username);
    }
}