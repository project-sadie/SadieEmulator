using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Locale;
using Sadie.Db;

namespace Sadie.Game.Locale;

public class LocaleService : ILocaleService
{
    private readonly Dictionary<string, string> _localeTexts;
    
    public LocaleService(IDbContextFactory<SadieDbContext> dbContextFactory)
    {
        using var db = dbContextFactory.CreateDbContext();
        
        _localeTexts = db.ServerLocaleTexts
            .ToDictionary(
                x => x.Key,
                x => x.Text);
    }

    public string this[string key]
    {
        get =>
            _localeTexts.TryGetValue(key, out var value)
                ? value
                : key;
        set => _localeTexts[key] = value;
    }
}