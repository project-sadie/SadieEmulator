using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Interfaces.Game.Locale;

namespace Sadie.Game.Locale;

public static class LocaleServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ILocaleService, LocaleService>();
    }
}