using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Game.Locale;

namespace Sadie.Game.Locale;

public static class LocaleServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ILocaleService, LocaleService>();
    }
}